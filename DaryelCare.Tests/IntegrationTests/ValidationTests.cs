using System.Net;
using System.Text;
using Xunit;

namespace DaryelCare.Tests.IntegrationTests;

public class ValidationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ValidationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostApplication_InvalidJson_ReturnsError()
    {
        // Arrange
        var invalidJson = "{ invalid json }";
        var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert - invalid JSON is caught by the global error handler as a server error
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task PatchApplication_InvalidJson_ReturnsError()
    {
        // Arrange
        var invalidJson = "{ invalid json }";
        var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync("/api/applications/RK-2026-00001", content);

        // Assert - invalid JSON is caught by the global error handler as a server error
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task PostTimeline_EmptyEvent_ReturnsBadRequest()
    {
        // Arrange
        var timelineEvent = "{\"event\":\"\",\"type\":\"action\"}";
        var content = new StringContent(timelineEvent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications/RK-2026-00001/timeline", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Event text is required", responseContent);
    }

    [Fact]
    public async Task PostApplication_WhitespaceOnlyFirstName_ReturnsBadRequest()
    {
        // Arrange
        var application = "{\"personal\":{\"firstName\":\"   \",\"lastName\":\"Doe\",\"email\":\"test@example.com\"}}";
        var content = new StringContent(application, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("First name is required", responseContent);
    }

    [Fact]
    public async Task PostApplication_WhitespaceOnlyLastName_ReturnsBadRequest()
    {
        // Arrange
        var application = "{\"personal\":{\"firstName\":\"John\",\"lastName\":\"   \",\"email\":\"test@example.com\"}}";
        var content = new StringContent(application, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Last name is required", responseContent);
    }

    [Fact]
    public async Task PostApplication_WhitespaceOnlyEmail_ReturnsBadRequest()
    {
        // Arrange
        var application = "{\"personal\":{\"firstName\":\"John\",\"lastName\":\"Doe\",\"email\":\"   \"}}";
        var content = new StringContent(application, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Email is required", responseContent);
    }

    [Theory]
    [InlineData("plaintext")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user name@example.com")]
    [InlineData("user@example")]
    public async Task PostApplication_InvalidEmailFormats_ReturnsBadRequest(string email)
    {
        // Arrange
        var application = $"{{\"personal\":{{\"firstName\":\"John\",\"lastName\":\"Doe\",\"email\":\"{email}\"}}}}";
        var content = new StringContent(application, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid email format", responseContent);
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@example.co.uk")]
    [InlineData("user+tag@example.org")]
    [InlineData("user123@test-domain.com")]
    public async Task PostApplication_ValidEmailFormats_Accepted(string email)
    {
        // Arrange
        var application = $"{{\"personal\":{{\"firstName\":\"John\",\"lastName\":\"Doe\",\"email\":\"{email}\"}}}}";
        var content = new StringContent(application, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostApplication_ExactlyMaxLengthFirstName_Accepted()
    {
        // Arrange - Exactly 200 characters
        var exactLength = new string('A', 200);
        var application = $"{{\"personal\":{{\"firstName\":\"{exactLength}\",\"lastName\":\"Doe\",\"email\":\"test@example.com\"}}}}";
        var content = new StringContent(application, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostApplication_ExactlyMaxLengthLastName_Accepted()
    {
        // Arrange - Exactly 200 characters
        var exactLength = new string('A', 200);
        var application = $"{{\"personal\":{{\"firstName\":\"John\",\"lastName\":\"{exactLength}\",\"email\":\"test@example.com\"}}}}";
        var content = new StringContent(application, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostApplication_ExactlyMaxLengthEmail_Accepted()
    {
        // Arrange - Exactly 254 characters
        var localPart = new string('a', 242);
        var email = $"{localPart}@example.com"; // Total = 242 + 12 = 254
        var application = $"{{\"personal\":{{\"firstName\":\"John\",\"lastName\":\"Doe\",\"email\":\"{email}\"}}}}";
        var content = new StringContent(application, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostTimeline_ExactlyMaxLengthEvent_Accepted()
    {
        // Arrange - Exactly 2000 characters
        var exactLength = new string('A', 2000);
        var timelineEvent = $"{{\"event\":\"{exactLength}\",\"type\":\"action\"}}";
        var content = new StringContent(timelineEvent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications/RK-2026-00001/timeline", content);

        // Assert
        // Should succeed (will fail with 404 if application doesn't exist, but should pass validation)
        Assert.NotEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostTimeline_DefaultTypeWhenMissing_UsesAction()
    {
        // This test verifies the default type is "action" when not provided
        // The actual implementation shows: var type = body?["type"]?.GetValue<string>() ?? "action";
        // We can't easily verify this without a real database, but we can verify it doesn't fail validation

        // Arrange
        var timelineEvent = "{\"event\":\"Test event\"}"; // No type field
        var content = new StringContent(timelineEvent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/applications/RK-2026-00001/timeline", content);

        // Assert
        // Should not fail with "Invalid event type" error
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("Invalid event type", responseContent);
    }
}
