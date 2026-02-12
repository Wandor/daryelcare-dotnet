using System.Text.Json;
using System.Text.Json.Nodes;
using DaryelCare.Data;
using DaryelCare.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddSingleton<ApplicationService>();

var app = builder.Build();

var port = app.Configuration.GetValue<int?>("Port") ?? 3000;
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

app.UseDefaultFiles();
app.UseStaticFiles();

// Seed command
if (args.Contains("--seed"))
{
    var seedDb = app.Services.GetRequiredService<DatabaseContext>();
    await seedDb.Seed();
    Console.WriteLine("Database seeded successfully.");
    return;
}

var svc = app.Services.GetRequiredService<ApplicationService>();

// ── API Routes ──

app.MapGet("/api/applications", async () =>
    Results.Ok(await svc.GetAllApplications()));

app.MapGet("/api/applications/{id}", async (string id) =>
{
    var result = await svc.GetApplication(id);
    return result is null ? Results.NotFound(new { error = "Application not found" }) : Results.Ok(result);
});

app.MapPost("/api/applications", async (HttpRequest req) =>
{
    using var reader = new StreamReader(req.Body);
    var json = await reader.ReadToEndAsync();
    var body = JsonNode.Parse(json);
    if (body is null)
        return Results.BadRequest(new { error = "Invalid JSON" });

    var id = await svc.CreateApplication(body);
    return Results.Created($"/api/applications/{id}", new { id, message = "Application submitted successfully" });
});

app.MapPatch("/api/applications/{id}", async (string id, HttpRequest req) =>
{
    using var reader = new StreamReader(req.Body);
    var json = await reader.ReadToEndAsync();
    var body = JsonNode.Parse(json);
    if (body is null)
        return Results.BadRequest(new { error = "Invalid JSON" });

    var updated = await svc.UpdateApplication(id, body);
    return updated
        ? Results.Ok(new { message = "Application updated" })
        : Results.NotFound(new { error = "Application not found or no valid fields" });
});

app.MapDelete("/api/applications/{id}", async (string id) =>
{
    var deleted = await svc.DeleteApplication(id);
    return deleted
        ? Results.Ok(new { message = "Application deleted" })
        : Results.NotFound(new { error = "Application not found" });
});

app.MapPost("/api/applications/{id}/timeline", async (string id, HttpRequest req) =>
{
    using var reader = new StreamReader(req.Body);
    var json = await reader.ReadToEndAsync();
    var body = JsonNode.Parse(json);
    var eventText = body?["event"]?.GetValue<string>();
    var type = body?["type"]?.GetValue<string>() ?? "action";

    if (string.IsNullOrEmpty(eventText))
        return Results.BadRequest(new { error = "Event text is required" });

    var entry = await svc.AddTimelineEvent(id, eventText, type);
    return Results.Created($"/api/applications/{id}/timeline", entry);
});

// ── HTML page routes ──
app.MapGet("/register", () => Results.Redirect("/childminder-registration-complete.html"));
app.MapGet("/admin", () => Results.Redirect("/cma-portal-v2.html"));

// Init schema on startup
var db = app.Services.GetRequiredService<DatabaseContext>();
await db.InitSchema();

app.Run();
