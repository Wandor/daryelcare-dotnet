using System.Text.Json;
using System.Text.Json.Nodes;
using Npgsql;
using NpgsqlTypes;
using DaryelCare.Data;

namespace DaryelCare.Services;

public class ApplicationService
{
    private readonly DatabaseContext _db;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ApplicationService(DatabaseContext db) => _db = db;

    private static string GenerateId(long seqVal)
    {
        var year = DateTime.Now.Year;
        return $"RK-{year}-{seqVal:D5}";
    }

    private static int CalculateProgress(JsonNode? checks)
    {
        if (checks is not JsonObject obj || obj.Count == 0) return 0;
        var total = obj.Count;
        var complete = obj.Count(kvp =>
            kvp.Value?["status"]?.GetValue<string>() == "complete");
        return (int)Math.Round((double)complete / total * 100);
    }

    private static JsonNode BuildChecksFromForm(JsonNode body)
    {
        var checks = new JsonObject
        {
            ["dbs"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
            ["dbs_update"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
            ["la_check"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
            ["ofsted"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
            ["gp_health"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
            ["ref_1"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
            ["ref_2"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
            ["first_aid"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
            ["safeguarding"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
            ["food_hygiene"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
            ["insurance"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
        };

        var suit = body["suitability"];
        if (suit?["hasDBS"]?.GetValue<string>() == "Yes" && suit["dbsNumber"] is { } dbsNum)
        {
            var today = DateTime.Today.ToString("yyyy-MM-dd");
            checks["dbs"] = new JsonObject
            {
                ["status"] = "pending", ["date"] = today,
                ["certificate"] = dbsNum.GetValue<string>(),
                ["details"] = "Certificate number provided on application"
            };
        }

        var quals = body["qualifications"];
        if (quals?["firstAidCompleted"]?.GetValue<string>() == "Yes")
            checks["first_aid"] = new JsonObject
            {
                ["status"] = "complete",
                ["date"] = quals["firstAidDate"]?.GetValue<string>(),
                ["provider"] = quals["firstAidOrg"]?.GetValue<string>()
            };
        if (quals?["safeguardingCompleted"]?.GetValue<string>() == "Yes")
            checks["safeguarding"] = new JsonObject
            {
                ["status"] = "complete",
                ["date"] = quals["safeguardingDate"]?.GetValue<string>(),
                ["provider"] = quals["safeguardingOrg"]?.GetValue<string>()
            };
        if (quals?["foodHygieneCompleted"]?.GetValue<string>() == "Yes")
            checks["food_hygiene"] = new JsonObject
            {
                ["status"] = "complete",
                ["date"] = quals["foodHygieneDate"]?.GetValue<string>(),
                ["provider"] = quals["foodHygieneOrg"]?.GetValue<string>()
            };

        var refs = body["references"];
        var todayStr = DateTime.Today.ToString("yyyy-MM-dd");
        if (refs?["ref1"]?["name"]?.GetValue<string>() is { Length: > 0 } r1Name)
            checks["ref_1"] = new JsonObject
            {
                ["status"] = "pending", ["date"] = todayStr, ["referee"] = r1Name,
                ["relationship"] = refs["ref1"]?["relationship"]?.GetValue<string>(),
                ["details"] = "Reference request to be sent"
            };
        if (refs?["ref2"]?["name"]?.GetValue<string>() is { Length: > 0 } r2Name)
            checks["ref_2"] = new JsonObject
            {
                ["status"] = "pending", ["date"] = todayStr, ["referee"] = r2Name,
                ["relationship"] = refs["ref2"]?["relationship"]?.GetValue<string>(),
                ["details"] = "Reference request to be sent"
            };

        return checks;
    }

    private static JsonArray BuildConnectedPersons(JsonNode body)
    {
        var persons = new JsonArray();
        var adults = body["household"]?["adults"]?.AsArray();
        if (adults == null) return persons;

        for (var i = 0; i < adults.Count; i++)
        {
            var a = adults[i];
            var first = a?["firstName"]?.GetValue<string>();
            var last = a?["lastName"]?.GetValue<string>();
            if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(last)) continue;

            persons.Add(new JsonObject
            {
                ["id"] = $"CP-NEW-{i + 1:D3}",
                ["name"] = $"{first} {last}",
                ["type"] = "household",
                ["relationship"] = a?["relationship"]?.GetValue<string>() ?? "Household member",
                ["dob"] = a?["dob"]?.GetValue<string>(),
                ["formStatus"] = "not-started",
                ["formType"] = "CMA-H2",
                ["checks"] = new JsonObject
                {
                    ["dbs"] = new JsonObject { ["status"] = "not-started", ["date"] = null },
                    ["la_check"] = new JsonObject { ["status"] = "not-started", ["date"] = null }
                }
            });
        }
        return persons;
    }

    private static string? BuildPremisesAddress(JsonNode body)
    {
        var premises = body["premises"];
        var pt = premises?["type"]?.GetValue<string>() ?? "Domestic";
        var sameAsHome = premises?["sameAsHome"];

        JsonNode? addr;
        if (pt == "Domestic" && sameAsHome?.GetValue<bool>() != false)
            addr = body["homeAddress"];
        else
            addr = premises?["address"];

        if (addr == null) return null;
        var parts = new[] { "line1", "line2", "town", "postcode" }
            .Select(k => addr[k]?.GetValue<string>())
            .Where(v => !string.IsNullOrEmpty(v));
        var result = string.Join(", ", parts);
        return string.IsNullOrEmpty(result) ? null : result;
    }

    private static NpgsqlParameter JsonbParam(string name, string? json)
    {
        var p = new NpgsqlParameter(name, NpgsqlDbType.Jsonb);
        p.Value = json is null ? DBNull.Value : json;
        return p;
    }

    private static string? NodeToString(JsonNode? node) =>
        node?.ToJsonString(JsonOpts);

    private static string FormatDate(object? val)
    {
        if (val is null || val is DBNull) return null!;
        if (val is DateTime dt) return dt.ToString("yyyy-MM-dd");
        if (val is DateOnly d) return d.ToString("yyyy-MM-dd");
        return val.ToString()![..10];
    }

    private static string? FormatDatetime(object? val)
    {
        if (val is null || val is DBNull) return null;
        if (val is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm");
        return val.ToString()?[..16];
    }

    private static JsonNode? ParseJsonb(object? val)
    {
        if (val is null || val is DBNull) return null;
        var str = val.ToString();
        return string.IsNullOrEmpty(str) ? null : JsonNode.Parse(str);
    }

    private static Dictionary<string, object?> ToDashboardShape(
        NpgsqlDataReader reader, List<Dictionary<string, object?>> timeline)
    {
        var now = DateTime.Now;
        var lastUpdated = reader["last_updated"] is DateTime lu ? lu : now;
        var daysInStage = Math.Max(0, (int)(now - lastUpdated).TotalDays);

        var result = new Dictionary<string, object?>
        {
            ["id"] = reader["id"],
            ["name"] = reader["name"],
            ["email"] = reader["email"],
            ["phone"] = reader["phone"] is DBNull ? "" : reader["phone"],
            ["dob"] = FormatDate(reader["dob"]),
            ["stage"] = reader["stage"],
            ["startDate"] = FormatDate(reader["start_date"]),
            ["registrationDate"] = FormatDate(reader["registration_date"]),
            ["lastUpdated"] = FormatDate(reader["last_updated"]),
            ["daysInStage"] = daysInStage,
            ["risk"] = reader["risk"],
            ["progress"] = reader["progress"],
            ["premisesType"] = reader["premises_type"] is DBNull ? "" : reader["premises_type"],
            ["premisesAddress"] = reader["premises_address"] is DBNull ? "" : reader["premises_address"],
            ["localAuthority"] = reader["local_authority"] is DBNull ? "" : reader["local_authority"],
            ["registers"] = ParseJsonb(reader["registers"]) ?? new JsonArray(),
            ["checks"] = ParseJsonb(reader["checks"]) ?? new JsonObject(),
            ["connectedPersons"] = ParseJsonb(reader["connected_persons"]) ?? new JsonArray(),
            ["timeline"] = timeline.Select(t => new Dictionary<string, object?>
            {
                ["date"] = FormatDatetime(t["created_at"]),
                ["event"] = t["event"],
                ["type"] = t["type"]
            }).Reverse().ToList()
        };

        if (reader["ni_number"] is not DBNull)
            result["niNumber"] = reader["ni_number"];
        if (reader["registration_number"] is not DBNull)
            result["registrationNumber"] = reader["registration_number"];
        if (reader["ofsted_check"] is not DBNull)
            result["ofstedCheck"] = ParseJsonb(reader["ofsted_check"]);
        if (reader["household"] is not DBNull)
            result["household"] = ParseJsonb(reader["household"]);
        if (reader["service"] is not DBNull)
            result["service"] = ParseJsonb(reader["service"]);
        if (reader["premises_details"] is not DBNull)
            result["premisesDetails"] = ParseJsonb(reader["premises_details"]);

        return result;
    }

    public async Task<string> CreateApplication(JsonNode body)
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();
        await using var tx = await conn.BeginTransactionAsync();

        await using var seqCmd = new NpgsqlCommand(
            "SELECT nextval('application_id_seq') AS val", conn, tx);
        var seqVal = (long)(await seqCmd.ExecuteScalarAsync())!;
        var id = GenerateId(seqVal);
        var now = DateTime.Now;

        var checks = BuildChecksFromForm(body);
        var connected = BuildConnectedPersons(body);
        var progress = CalculateProgress(checks);
        var premisesAddr = BuildPremisesAddress(body);

        var registers = body["service"]?["ageGroups"]?.ToJsonString() ?? "[]";
        var personal = body["personal"];
        var premises = body["premises"];

        var premisesDetails = new JsonObject
        {
            ["sameAsHome"] = premises?["sameAsHome"]?.DeepClone(),
            ["outdoorSpace"] = premises?["outdoorSpace"]?.DeepClone(),
            ["pets"] = premises?["pets"]?.DeepClone(),
            ["petsDetails"] = premises?["petsDetails"]?.DeepClone()
        };

        const string sql = """
            INSERT INTO applications (
                id, title, first_name, middle_names, last_name,
                email, phone, dob, gender, right_to_work, ni_number,
                home_address, premises_type, premises_address,
                premises_details, local_authority,
                registers, service, stage, risk, progress,
                checks, connected_persons,
                previous_names, address_history, qualifications,
                employment_history, references_data,
                household, suitability, declaration,
                start_date, last_updated, created_at
            ) VALUES (
                @id, @title, @firstName, @middleNames, @lastName,
                @email, @phone, @dob, @gender, @rightToWork, @niNumber,
                @homeAddress, @premisesType, @premisesAddress,
                @premisesDetails, @localAuthority,
                @registers, @service, 'new', 'low', @progress,
                @checks, @connectedPersons,
                @previousNames, @addressHistory, @qualifications,
                @employmentHistory, @referencesData,
                @household, @suitability, @declaration,
                @startDate, @lastUpdated, @createdAt
            )
            """;

        await using var cmd = new NpgsqlCommand(sql, conn, tx);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("title", (object?)personal?["title"]?.GetValue<string>() ?? DBNull.Value);
        cmd.Parameters.AddWithValue("firstName", personal?["firstName"]?.GetValue<string>() ?? "");
        cmd.Parameters.AddWithValue("middleNames", (object?)personal?["middleNames"]?.GetValue<string>() ?? DBNull.Value);
        cmd.Parameters.AddWithValue("lastName", personal?["lastName"]?.GetValue<string>() ?? "");
        cmd.Parameters.AddWithValue("email", personal?["email"]?.GetValue<string>() ?? "");
        cmd.Parameters.AddWithValue("phone", (object?)personal?["phone"]?.GetValue<string>() ?? DBNull.Value);
        cmd.Parameters.AddWithValue("dob", (object?)personal?["dob"]?.GetValue<string>() ?? DBNull.Value);
        cmd.Parameters.AddWithValue("gender", (object?)personal?["gender"]?.GetValue<string>() ?? DBNull.Value);
        cmd.Parameters.AddWithValue("rightToWork", (object?)personal?["rightToWork"]?.GetValue<string>() ?? DBNull.Value);
        cmd.Parameters.AddWithValue("niNumber", (object?)personal?["niNumber"]?.GetValue<string>() ?? DBNull.Value);
        cmd.Parameters.Add(JsonbParam("homeAddress", NodeToString(body["homeAddress"]) ?? "{}"));
        cmd.Parameters.AddWithValue("premisesType", (premises?["type"]?.GetValue<string>() ?? "domestic").ToLower());
        cmd.Parameters.AddWithValue("premisesAddress", (object?)premisesAddr ?? DBNull.Value);
        cmd.Parameters.Add(JsonbParam("premisesDetails", premisesDetails.ToJsonString()));
        cmd.Parameters.AddWithValue("localAuthority", (object?)premises?["localAuthority"]?.GetValue<string>() ?? DBNull.Value);
        cmd.Parameters.Add(JsonbParam("registers", registers));
        cmd.Parameters.Add(JsonbParam("service", NodeToString(body["service"])));
        cmd.Parameters.AddWithValue("progress", progress);
        cmd.Parameters.Add(JsonbParam("checks", checks.ToJsonString()));
        cmd.Parameters.Add(JsonbParam("connectedPersons", connected.ToJsonString()));
        cmd.Parameters.Add(JsonbParam("previousNames", NodeToString(body["previousNames"])));
        cmd.Parameters.Add(JsonbParam("addressHistory", NodeToString(body["addressHistory"])));
        cmd.Parameters.Add(JsonbParam("qualifications", NodeToString(body["qualifications"])));
        cmd.Parameters.Add(JsonbParam("employmentHistory", NodeToString(body["employment"])));
        cmd.Parameters.Add(JsonbParam("referencesData", NodeToString(body["references"])));
        cmd.Parameters.Add(JsonbParam("household", NodeToString(body["household"])));
        cmd.Parameters.Add(JsonbParam("suitability", NodeToString(body["suitability"])));
        cmd.Parameters.Add(JsonbParam("declaration", NodeToString(body["declaration"])));
        cmd.Parameters.AddWithValue("startDate", now);
        cmd.Parameters.AddWithValue("lastUpdated", now);
        cmd.Parameters.AddWithValue("createdAt", now);
        await cmd.ExecuteNonQueryAsync();

        await using var tl1 = new NpgsqlCommand(
            "INSERT INTO timeline_events (application_id, event, type, created_at) VALUES (@id, 'Application started', 'action', @dt)",
            conn, tx);
        tl1.Parameters.AddWithValue("id", id);
        tl1.Parameters.AddWithValue("dt", now);
        await tl1.ExecuteNonQueryAsync();

        await using var tl2 = new NpgsqlCommand(
            "INSERT INTO timeline_events (application_id, event, type, created_at) VALUES (@id, 'Application form submitted', 'complete', @dt)",
            conn, tx);
        tl2.Parameters.AddWithValue("id", id);
        tl2.Parameters.AddWithValue("dt", now.AddSeconds(1));
        await tl2.ExecuteNonQueryAsync();

        await tx.CommitAsync();
        return id;
    }

    public async Task<List<Dictionary<string, object?>>> GetAllApplications()
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();

        var results = new List<Dictionary<string, object?>>();
        await using var cmd = new NpgsqlCommand(
            "SELECT * FROM applications ORDER BY created_at DESC", conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        var apps = new List<(string id, Dictionary<string, object?> shape)>();
        while (await reader.ReadAsync())
        {
            var timeline = new List<Dictionary<string, object?>>();
            apps.Add((reader["id"].ToString()!, ToDashboardShape(reader, timeline)));
        }
        await reader.CloseAsync();

        foreach (var (appId, shape) in apps)
        {
            await using var tlCmd = new NpgsqlCommand(
                "SELECT event, type, created_at FROM timeline_events WHERE application_id = @id ORDER BY created_at ASC",
                conn);
            tlCmd.Parameters.AddWithValue("id", appId);
            var timeline = new List<Dictionary<string, object?>>();
            await using var tlReader = await tlCmd.ExecuteReaderAsync();
            while (await tlReader.ReadAsync())
            {
                timeline.Add(new Dictionary<string, object?>
                {
                    ["event"] = tlReader["event"],
                    ["type"] = tlReader["type"],
                    ["created_at"] = tlReader["created_at"]
                });
            }
            await tlReader.CloseAsync();

            shape["timeline"] = timeline.Select(t => new Dictionary<string, object?>
            {
                ["date"] = FormatDatetime(t["created_at"]),
                ["event"] = t["event"],
                ["type"] = t["type"]
            }).Reverse().ToList();

            results.Add(shape);
        }

        return results;
    }

    public async Task<Dictionary<string, object?>?> GetApplication(string id)
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT * FROM applications WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        var timeline = new List<Dictionary<string, object?>>();
        var shape = ToDashboardShape(reader, timeline);
        await reader.CloseAsync();

        await using var tlCmd = new NpgsqlCommand(
            "SELECT event, type, created_at FROM timeline_events WHERE application_id = @id ORDER BY created_at ASC",
            conn);
        tlCmd.Parameters.AddWithValue("id", id);
        await using var tlReader = await tlCmd.ExecuteReaderAsync();
        while (await tlReader.ReadAsync())
        {
            timeline.Add(new Dictionary<string, object?>
            {
                ["event"] = tlReader["event"],
                ["type"] = tlReader["type"],
                ["created_at"] = tlReader["created_at"]
            });
        }

        shape["timeline"] = timeline.Select(t => new Dictionary<string, object?>
        {
            ["date"] = FormatDatetime(t["created_at"]),
            ["event"] = t["event"],
            ["type"] = t["type"]
        }).Reverse().ToList();

        return shape;
    }

    public async Task<bool> UpdateApplication(string id, JsonNode updates)
    {
        var allowed = new Dictionary<string, (string col, bool isJson)>
        {
            ["stage"] = ("stage", false),
            ["risk"] = ("risk", false),
            ["progress"] = ("progress", false),
            ["checks"] = ("checks", true),
            ["connected_persons"] = ("connected_persons", true),
            ["connectedPersons"] = ("connected_persons", true),
            ["ofsted_check"] = ("ofsted_check", true),
            ["ofstedCheck"] = ("ofsted_check", true),
            ["registration_date"] = ("registration_date", false),
            ["registrationDate"] = ("registration_date", false),
            ["registration_number"] = ("registration_number", false),
            ["registrationNumber"] = ("registration_number", false),
        };

        var sets = new List<string>();
        var parameters = new List<NpgsqlParameter>();
        var idx = 1;

        if (updates is JsonObject obj)
        {
            foreach (var kvp in obj)
            {
                if (!allowed.TryGetValue(kvp.Key, out var mapping)) continue;
                var paramName = $"@p{idx}";
                sets.Add($"{mapping.col} = {paramName}");
                if (mapping.isJson)
                    parameters.Add(JsonbParam($"p{idx}", kvp.Value?.ToJsonString()));
                else
                    parameters.Add(new NpgsqlParameter($"p{idx}", (object?)kvp.Value?.GetValue<string>() ?? DBNull.Value));
                idx++;
            }
        }

        if (sets.Count == 0) return false;

        sets.Add("last_updated = NOW()");
        var sql = $"UPDATE applications SET {string.Join(", ", sets)} WHERE id = @id";

        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        foreach (var p in parameters) cmd.Parameters.Add(p);
        cmd.Parameters.AddWithValue("id", id);
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> DeleteApplication(string id)
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "DELETE FROM applications WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<Dictionary<string, object?>> AddTimelineEvent(
        string applicationId, string eventText, string type = "action")
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(
            "INSERT INTO timeline_events (application_id, event, type) VALUES (@appId, @event, @type) RETURNING *",
            conn);
        cmd.Parameters.AddWithValue("appId", applicationId);
        cmd.Parameters.AddWithValue("event", eventText);
        cmd.Parameters.AddWithValue("type", type);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return new Dictionary<string, object?>
        {
            ["id"] = reader["id"],
            ["application_id"] = reader["application_id"],
            ["event"] = reader["event"],
            ["type"] = reader["type"],
            ["created_at"] = reader["created_at"]
        };
    }
}
