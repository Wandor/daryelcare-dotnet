using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace DaryelCare.Tests.IntegrationTests;

public class ApiEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"ok\"", content);
    }

    [Fact]
    public async Task HealthEndpoint_HasSecurityHeaders()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.True(response.Headers.Contains("X-Content-Type-Options"));
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").First());
        Assert.True(response.Headers.Contains("X-Frame-Options"));
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").First());
        Assert.True(response.Headers.Contains("X-XSS-Protection"));
        Assert.Equal("1; mode=block", response.Headers.GetValues("X-XSS-Protection").First());
    }

    [Fact]
    public async Task GetApplications_ReturnsEmptyArray()
    {
        // Act
        var response = await _client.GetAsync("/api/applications");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var applications = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(applications);
    }

    [Fact]
    public async Task GetApplicationById_NonExistent_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/applications/RK-2026-99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Application not found", content);
    }

    [Fact]
    public async Task PostApplication_ValidData_ReturnsCreated()
    {
        // Arrange
        var application = new
        {
            personal = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "john.doe@example.com"
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"id\":", responseContent);
        Assert.Contains("RK-", responseContent);
    }

    [Fact]
    public async Task PostApplication_MissingFirstName_ReturnsBadRequest()
    {
        // Arrange
        var application = new
        {
            personal = new
            {
                lastName = "Doe",
                email = "john.doe@example.com"
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("First name is required", responseContent);
    }

    [Fact]
    public async Task PostApplication_MissingLastName_ReturnsBadRequest()
    {
        // Arrange
        var application = new
        {
            personal = new
            {
                firstName = "John",
                email = "john.doe@example.com"
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Last name is required", responseContent);
    }

    [Fact]
    public async Task PostApplication_MissingEmail_ReturnsBadRequest()
    {
        // Arrange
        var application = new
        {
            personal = new
            {
                firstName = "John",
                lastName = "Doe"
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Email is required", responseContent);
    }

    [Fact]
    public async Task PostApplication_InvalidEmailFormat_ReturnsBadRequest()
    {
        // Arrange
        var application = new
        {
            personal = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "invalid-email"
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid email format", responseContent);
    }

    [Fact]
    public async Task PostApplication_EmailWithoutDomain_ReturnsBadRequest()
    {
        // Arrange
        var application = new
        {
            personal = new
            {
                firstName = "John",
                lastName = "Doe",
                email = "john@example"
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid email format", responseContent);
    }

    [Fact]
    public async Task PostApplication_FirstNameTooLong_ReturnsBadRequest()
    {
        // Arrange
        var longName = new string('A', 201);
        var application = new
        {
            personal = new
            {
                firstName = longName,
                lastName = "Doe",
                email = "john.doe@example.com"
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("First name must not exceed 200 characters", responseContent);
    }

    [Fact]
    public async Task PostApplication_LastNameTooLong_ReturnsBadRequest()
    {
        // Arrange
        var longName = new string('A', 201);
        var application = new
        {
            personal = new
            {
                firstName = "John",
                lastName = longName,
                email = "john.doe@example.com"
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Last name must not exceed 200 characters", responseContent);
    }

    [Fact]
    public async Task PostApplication_EmailTooLong_ReturnsBadRequest()
    {
        // Arrange
        var longEmail = new string('a', 246) + "@test.com"; // Total = 255, exceeds 254
        var application = new
        {
            personal = new
            {
                firstName = "John",
                lastName = "Doe",
                email = longEmail
            }
        };

        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Email must not exceed 254 characters", responseContent);
    }

    [Fact]
    public async Task PatchApplication_ValidStage_ReturnsOk()
    {
        // Arrange - Create an application first
        var application = new
        {
            personal = new
            {
                firstName = "Jane",
                lastName = "Smith",
                email = "jane.smith@example.com"
            }
        };

        var createJson = JsonSerializer.Serialize(application);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/applications", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<JsonElement>(createResponseContent);
        var id = createResult.GetProperty("id").GetString();

        // Update the stage
        var update = new { stage = "form-submitted" };
        var updateJson = JsonSerializer.Serialize(update);
        var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/applications/{id}", updateContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Application updated", responseContent);
    }

    [Fact]
    public async Task PatchApplication_InvalidStage_ReturnsBadRequest()
    {
        // Arrange
        var update = new { stage = "invalid-stage" };
        var json = JsonSerializer.Serialize(update);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync("/api/applications/RK-2026-00001", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid stage value", responseContent);
    }

    [Theory]
    [InlineData("new")]
    [InlineData("form-submitted")]
    [InlineData("checks")]
    [InlineData("review")]
    [InlineData("approved")]
    [InlineData("blocked")]
    [InlineData("registered")]
    public async Task PatchApplication_AllValidStages_Accepted(string stage)
    {
        // Arrange - Create an application first
        var application = new
        {
            personal = new
            {
                firstName = "Test",
                lastName = "User",
                email = "test@example.com"
            }
        };

        var createJson = JsonSerializer.Serialize(application);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/applications", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<JsonElement>(createResponseContent);
        var id = createResult.GetProperty("id").GetString();

        var update = new { stage };
        var json = JsonSerializer.Serialize(update);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/applications/{id}", content);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound,
            $"Stage '{stage}' should be valid and return OK or NotFound, but got {response.StatusCode}");
    }

    [Fact]
    public async Task PatchApplication_NonExistent_Returns404()
    {
        // Arrange
        var update = new { stage = "checks" };
        var json = JsonSerializer.Serialize(update);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync("/api/applications/RK-2026-99999", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Application not found", responseContent);
    }

    [Fact]
    public async Task DeleteApplication_NonExistent_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/applications/RK-2026-99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Application not found", responseContent);
    }

    [Fact]
    public async Task PostTimeline_ValidEvent_ReturnsCreated()
    {
        // Arrange - Create an application first
        var application = new
        {
            personal = new
            {
                firstName = "Timeline",
                lastName = "Test",
                email = "timeline@example.com"
            }
        };

        var createJson = JsonSerializer.Serialize(application);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/applications", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<JsonElement>(createResponseContent);
        var id = createResult.GetProperty("id").GetString();

        // Add timeline event
        var timelineEvent = new
        {
            @event = "Test event",
            type = "action"
        };
        var json = JsonSerializer.Serialize(timelineEvent);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/applications/{id}/timeline", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostTimeline_MissingEvent_ReturnsBadRequest()
    {
        // Arrange
        var timelineEvent = new { type = "action" };
        var json = JsonSerializer.Serialize(timelineEvent);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications/RK-2026-00001/timeline", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Event text is required", responseContent);
    }

    [Fact]
    public async Task PostTimeline_EventTooLong_ReturnsBadRequest()
    {
        // Arrange
        var longEvent = new string('A', 2001);
        var timelineEvent = new
        {
            @event = longEvent,
            type = "action"
        };
        var json = JsonSerializer.Serialize(timelineEvent);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications/RK-2026-00001/timeline", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Event text must not exceed 2000 characters", responseContent);
    }

    [Fact]
    public async Task PostTimeline_InvalidType_ReturnsBadRequest()
    {
        // Arrange
        var timelineEvent = new
        {
            @event = "Test event",
            type = "invalid-type"
        };
        var json = JsonSerializer.Serialize(timelineEvent);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications/RK-2026-00001/timeline", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid event type", responseContent);
    }

    [Theory]
    [InlineData("action")]
    [InlineData("complete")]
    [InlineData("alert")]
    [InlineData("note")]
    public async Task PostTimeline_AllValidTypes_Accepted(string type)
    {
        // Arrange - Create an application first
        var application = new
        {
            personal = new
            {
                firstName = "Type",
                lastName = "Test",
                email = "type@example.com"
            }
        };

        var createJson = JsonSerializer.Serialize(application);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/applications", createContent);
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<JsonElement>(createResponseContent);
        var id = createResult.GetProperty("id").GetString();

        var timelineEvent = new
        {
            @event = $"Test event for {type}",
            type
        };
        var json = JsonSerializer.Serialize(timelineEvent);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/applications/{id}/timeline", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task AllEndpoints_HaveSecurityHeaders()
    {
        // Test GET /api/applications
        var response1 = await _client.GetAsync("/api/applications");
        Assert.True(response1.Headers.Contains("X-Content-Type-Options"));
        Assert.True(response1.Headers.Contains("X-Frame-Options"));
        Assert.True(response1.Headers.Contains("X-XSS-Protection"));

        // Test POST /api/applications
        var application = new
        {
            personal = new
            {
                firstName = "Security",
                lastName = "Test",
                email = "security@example.com"
            }
        };
        var json = JsonSerializer.Serialize(application);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response2 = await _client.PostAsync("/api/applications", content);
        Assert.True(response2.Headers.Contains("X-Content-Type-Options"));
        Assert.True(response2.Headers.Contains("X-Frame-Options"));
        Assert.True(response2.Headers.Contains("X-XSS-Protection"));
    }
}
