using System.Text.Json.Nodes;
using DaryelCare.Services;

namespace DaryelCare.Tests.Mocks;

public class MockApplicationService : ApplicationService
{
    private readonly Dictionary<string, Dictionary<string, object?>> _applications = new();
    private readonly Dictionary<string, List<Dictionary<string, object?>>> _timelines = new();
    private int _idCounter = 1;

    public MockApplicationService() : base(null!)
    {
    }

    public override async Task<string> CreateApplication(JsonNode body)
    {
        await Task.CompletedTask;
        var id = $"RK-{DateTime.Now.Year}-{_idCounter:D5}";
        _idCounter++;

        var personal = body["personal"];
        var application = new Dictionary<string, object?>
        {
            ["id"] = id,
            ["name"] = $"{personal?["firstName"]?.GetValue<string>()} {personal?["lastName"]?.GetValue<string>()}",
            ["email"] = personal?["email"]?.GetValue<string>(),
            ["stage"] = "new",
            ["timeline"] = new List<Dictionary<string, object?>>()
        };

        _applications[id] = application;
        _timelines[id] = new List<Dictionary<string, object?>>();

        return id;
    }

    public override async Task<List<Dictionary<string, object?>>> GetAllApplications()
    {
        await Task.CompletedTask;
        return _applications.Values.ToList();
    }

    public override async Task<Dictionary<string, object?>?> GetApplication(string id)
    {
        await Task.CompletedTask;
        return _applications.TryGetValue(id, out var app) ? app : null;
    }

    public override async Task<bool> UpdateApplication(string id, JsonNode updates)
    {
        await Task.CompletedTask;
        if (!_applications.ContainsKey(id))
            return false;

        if (updates is JsonObject obj)
        {
            foreach (var kvp in obj)
            {
                if (kvp.Key == "stage")
                    _applications[id]["stage"] = kvp.Value?.GetValue<string>();
            }
        }

        return true;
    }

    public override async Task<bool> DeleteApplication(string id)
    {
        await Task.CompletedTask;
        return _applications.Remove(id);
    }

    public override async Task<Dictionary<string, object?>> AddTimelineEvent(
        string applicationId, string eventText, string type = "action")
    {
        await Task.CompletedTask;
        if (!_timelines.ContainsKey(applicationId))
            _timelines[applicationId] = new List<Dictionary<string, object?>>();

        var entry = new Dictionary<string, object?>
        {
            ["id"] = _timelines[applicationId].Count + 1,
            ["application_id"] = applicationId,
            ["event"] = eventText,
            ["type"] = type,
            ["created_at"] = DateTime.Now
        };

        _timelines[applicationId].Add(entry);
        return entry;
    }
}
