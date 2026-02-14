using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace DaryelCare.Tests.IntegrationTests;

public class CrudOperationsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CrudOperationsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAndRetrieveApplication_Success()
    {
        // Arrange
        var application = new
        {
            personal = new
            {
                firstName = "Alice",
                lastName = "Johnson",
                email = "alice.johnson@example.com"
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act - Create
        var createResponse = await _client.PostAsync("/api/applications", content);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<JsonElement>(createContent);
        var id = createResult.GetProperty("id").GetString();

        // Act - Retrieve
        var getResponse = await _client.GetAsync($"/api/applications/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getContent = await getResponse.Content.ReadAsStringAsync();
        var getResult = JsonSerializer.Deserialize<JsonElement>(getContent);
        Assert.Equal(id, getResult.GetProperty("id").GetString());
        Assert.Equal("alice.johnson@example.com", getResult.GetProperty("email").GetString());
    }

    [Fact]
    public async Task CreateUpdateAndRetrieveApplication_Success()
    {
        // Arrange - Create application
        var application = new
        {
            personal = new
            {
                firstName = "Bob",
                lastName = "Smith",
                email = "bob.smith@example.com"
            }
        };

        var createJson = JsonSerializer.Serialize(application);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/applications", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<JsonElement>(createResponseContent);
        var id = createResult.GetProperty("id").GetString();

        // Act - Update stage
        var update = new { stage = "checks" };
        var updateJson = JsonSerializer.Serialize(update);
        var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");
        var updateResponse = await _client.PatchAsync($"/api/applications/{id}", updateContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Verify update
        var getResponse = await _client.GetAsync($"/api/applications/{id}");
        var getContent = await getResponse.Content.ReadAsStringAsync();
        var getResult = JsonSerializer.Deserialize<JsonElement>(getContent);
        Assert.Equal("checks", getResult.GetProperty("stage").GetString());
    }

    [Fact]
    public async Task CreateDeleteAndRetrieveApplication_Returns404()
    {
        // Arrange - Create application
        var application = new
        {
            personal = new
            {
                firstName = "Charlie",
                lastName = "Brown",
                email = "charlie.brown@example.com"
            }
        };

        var createJson = JsonSerializer.Serialize(application);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/applications", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<JsonElement>(createResponseContent);
        var id = createResult.GetProperty("id").GetString();

        // Act - Delete
        var deleteResponse = await _client.DeleteAsync($"/api/applications/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/applications/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateApplicationAndAddTimelineEvent_Success()
    {
        // Arrange - Create application
        var application = new
        {
            personal = new
            {
                firstName = "Diana",
                lastName = "Prince",
                email = "diana.prince@example.com"
            }
        };

        var createJson = JsonSerializer.Serialize(application);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/applications", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<JsonElement>(createResponseContent);
        var id = createResult.GetProperty("id").GetString();

        // Act - Add timeline event
        var timelineEvent = new
        {
            @event = "DBS check completed",
            type = "complete"
        };
        var timelineJson = JsonSerializer.Serialize(timelineEvent);
        var timelineContent = new StringContent(timelineJson, Encoding.UTF8, "application/json");
        var timelineResponse = await _client.PostAsync($"/api/applications/{id}/timeline", timelineContent);

        // Assert
        Assert.Equal(HttpStatusCode.Created, timelineResponse.StatusCode);
        var timelineResponseContent = await timelineResponse.Content.ReadAsStringAsync();
        var timelineResult = JsonSerializer.Deserialize<JsonElement>(timelineResponseContent);
        Assert.Equal("DBS check completed", timelineResult.GetProperty("event").GetString());
        Assert.Equal("complete", timelineResult.GetProperty("type").GetString());
    }

    [Fact]
    public async Task UpdateNonExistentApplication_Returns404()
    {
        // Arrange
        var update = new { stage = "approved" };
        var json = JsonSerializer.Serialize(update);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync("/api/applications/RK-9999-99999", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNonExistentApplication_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/applications/RK-9999-99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MultipleApplications_GetAllReturnsAll()
    {
        // Arrange - Create multiple applications
        var names = new[] { "Emma", "Frank", "Grace" };
        foreach (var name in names)
        {
            var application = new
            {
                personal = new
                {
                    firstName = name,
                    lastName = "Test",
                    email = $"{name.ToLower()}@example.com"
                }
            };

            var json = JsonSerializer.Serialize(application);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _client.PostAsync("/api/applications", content);
        }

        // Act
        var response = await _client.GetAsync("/api/applications");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var applications = JsonSerializer.Deserialize<JsonElement[]>(responseContent);
        Assert.NotNull(applications);
        Assert.True(applications.Length >= 3);
    }

    [Fact]
    public async Task StageProgression_ThroughAllStages()
    {
        // Arrange - Create application
        var application = new
        {
            personal = new
            {
                firstName = "Henry",
                lastName = "Wilson",
                email = "henry.wilson@example.com"
            }
        };

        var createJson = JsonSerializer.Serialize(application);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/applications", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<JsonElement>(createResponseContent);
        var id = createResult.GetProperty("id").GetString();

        // Act & Assert - Progress through all stages
        var stages = new[] { "form-submitted", "checks", "review", "approved", "registered" };
        foreach (var stage in stages)
        {
            var update = new { stage };
            var updateJson = JsonSerializer.Serialize(update);
            var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");
            var updateResponse = await _client.PatchAsync($"/api/applications/{id}", updateContent);

            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            // Verify stage was updated
            var getResponse = await _client.GetAsync($"/api/applications/{id}");
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var getResult = JsonSerializer.Deserialize<JsonElement>(getContent);
            Assert.Equal(stage, getResult.GetProperty("stage").GetString());
        }
    }

    [Fact]
    public async Task CreateApplication_LocationHeaderContainsId()
    {
        // Arrange
        var application = new
        {
            personal = new
            {
                firstName = "Iris",
                lastName = "Taylor",
                email = "iris.taylor@example.com"
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        var id = result.GetProperty("id").GetString();

        Assert.NotNull(response.Headers.Location);
        Assert.Contains(id, response.Headers.Location.ToString());
    }
}
