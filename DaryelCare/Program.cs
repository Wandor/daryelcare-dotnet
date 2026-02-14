using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using DaryelCare.Data;
using DaryelCare.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddSingleton<ApplicationService>();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.WebHost.ConfigureKestrel(options =>
    options.Limits.MaxRequestBodySize = 1_048_576);

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandler = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (exceptionHandler != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exceptionHandler.Error, "Unhandled exception occurred");
        }

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\":\"An error occurred processing your request\"}");
    });
});

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    await next();
});

app.UseCors();

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

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

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

    var personal = body["personal"];
    var firstName = personal?["firstName"]?.GetValue<string>();
    var lastName = personal?["lastName"]?.GetValue<string>();
    var email = personal?["email"]?.GetValue<string>();

    if (string.IsNullOrWhiteSpace(firstName))
        return Results.BadRequest(new { error = "First name is required" });
    if (string.IsNullOrWhiteSpace(lastName))
        return Results.BadRequest(new { error = "Last name is required" });
    if (string.IsNullOrWhiteSpace(email))
        return Results.BadRequest(new { error = "Email is required" });

    if (firstName.Length > 200)
        return Results.BadRequest(new { error = "First name must not exceed 200 characters" });
    if (lastName.Length > 200)
        return Results.BadRequest(new { error = "Last name must not exceed 200 characters" });
    if (email.Length > 254)
        return Results.BadRequest(new { error = "Email must not exceed 254 characters" });

    var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    if (!emailRegex.IsMatch(email))
        return Results.BadRequest(new { error = "Invalid email format" });

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

    if (body["stage"] is JsonNode stageNode)
    {
        var stage = stageNode.GetValue<string>();
        var validStages = new[] { "new", "form-submitted", "checks", "review", "approved", "blocked", "registered" };
        if (!validStages.Contains(stage))
            return Results.BadRequest(new { error = "Invalid stage value" });
    }

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

    if (eventText.Length > 2000)
        return Results.BadRequest(new { error = "Event text must not exceed 2000 characters" });

    var validTypes = new[] { "action", "complete", "alert", "note" };
    if (!validTypes.Contains(type))
        return Results.BadRequest(new { error = "Invalid event type" });

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

public partial class Program { }
