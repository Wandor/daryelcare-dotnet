using System.Text.Json.Nodes;
using DaryelCare.Services;
using Xunit;

namespace DaryelCare.Tests.UnitTests;

public class ApplicationServiceTests
{
    [Theory]
    [InlineData("Hello World", "Hello World")]
    [InlineData("<script>alert('XSS')</script>", "&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;")]
    [InlineData("John & Jane", "John &amp; Jane")]
    [InlineData("Test \"quotes\"", "Test &quot;quotes&quot;")]
    [InlineData("Normal text 123", "Normal text 123")]
    [InlineData("<b>Bold</b>", "&lt;b&gt;Bold&lt;/b&gt;")]
    [InlineData("", "")]
    public void EscapeHtml_SanitizesInput(string input, string expected)
    {
        // Act
        var result = ApplicationService.EscapeHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GenerateId_CreatesCorrectFormat()
    {
        // Arrange
        var currentYear = DateTime.Now.Year;
        long seqVal = 123;

        // Act
        var result = ApplicationService.GenerateId(seqVal);

        // Assert
        Assert.StartsWith($"RK-{currentYear}-", result);
        Assert.Equal($"RK-{currentYear}-00123", result);
    }

    [Theory]
    [InlineData(1, "RK-2026-00001")]
    [InlineData(42, "RK-2026-00042")]
    [InlineData(999, "RK-2026-00999")]
    [InlineData(12345, "RK-2026-12345")]
    [InlineData(99999, "RK-2026-99999")]
    public void GenerateId_FormatsWithLeadingZeros(long seqVal, string expectedSuffix)
    {
        // Act
        var result = ApplicationService.GenerateId(seqVal);

        // Assert
        var currentYear = DateTime.Now.Year;
        Assert.Equal($"RK-{currentYear}-{seqVal:D5}", result);
    }

    [Fact]
    public void CalculateProgress_EmptyChecks_ReturnsZero()
    {
        // Arrange
        var checks = new JsonObject();

        // Act
        var result = ApplicationService.CalculateProgress(checks);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateProgress_NullChecks_ReturnsZero()
    {
        // Act
        var result = ApplicationService.CalculateProgress(null);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateProgress_AllIncomplete_ReturnsZero()
    {
        // Arrange
        var checks = new JsonObject
        {
            ["dbs"] = new JsonObject { ["status"] = "not-started" },
            ["la_check"] = new JsonObject { ["status"] = "pending" },
            ["ofsted"] = new JsonObject { ["status"] = "in-progress" }
        };

        // Act
        var result = ApplicationService.CalculateProgress(checks);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateProgress_AllComplete_Returns100()
    {
        // Arrange
        var checks = new JsonObject
        {
            ["dbs"] = new JsonObject { ["status"] = "complete" },
            ["la_check"] = new JsonObject { ["status"] = "complete" },
            ["ofsted"] = new JsonObject { ["status"] = "complete" }
        };

        // Act
        var result = ApplicationService.CalculateProgress(checks);

        // Assert
        Assert.Equal(100, result);
    }

    [Fact]
    public void CalculateProgress_HalfComplete_Returns50()
    {
        // Arrange
        var checks = new JsonObject
        {
            ["dbs"] = new JsonObject { ["status"] = "complete" },
            ["la_check"] = new JsonObject { ["status"] = "complete" },
            ["ofsted"] = new JsonObject { ["status"] = "not-started" },
            ["gp_health"] = new JsonObject { ["status"] = "pending" }
        };

        // Act
        var result = ApplicationService.CalculateProgress(checks);

        // Assert
        Assert.Equal(50, result);
    }

    [Fact]
    public void CalculateProgress_OneOfThreeComplete_Returns33()
    {
        // Arrange
        var checks = new JsonObject
        {
            ["dbs"] = new JsonObject { ["status"] = "complete" },
            ["la_check"] = new JsonObject { ["status"] = "not-started" },
            ["ofsted"] = new JsonObject { ["status"] = "pending" }
        };

        // Act
        var result = ApplicationService.CalculateProgress(checks);

        // Assert
        Assert.Equal(33, result); // Rounds to 33
    }

    [Fact]
    public void CalculateProgress_TwoOfThreeComplete_Returns67()
    {
        // Arrange
        var checks = new JsonObject
        {
            ["dbs"] = new JsonObject { ["status"] = "complete" },
            ["la_check"] = new JsonObject { ["status"] = "complete" },
            ["ofsted"] = new JsonObject { ["status"] = "pending" }
        };

        // Act
        var result = ApplicationService.CalculateProgress(checks);

        // Assert
        Assert.Equal(67, result); // Rounds to 67
    }

    [Fact]
    public void CalculateProgress_MissingStatusField_TreatsAsIncomplete()
    {
        // Arrange
        var checks = new JsonObject
        {
            ["dbs"] = new JsonObject { ["status"] = "complete" },
            ["la_check"] = new JsonObject { ["date"] = "2026-01-01" }, // No status field
            ["ofsted"] = new JsonObject { ["status"] = "complete" }
        };

        // Act
        var result = ApplicationService.CalculateProgress(checks);

        // Assert
        Assert.Equal(67, result); // 2 out of 3 complete
    }

    [Fact]
    public void CalculateProgress_NullStatusValue_TreatsAsIncomplete()
    {
        // Arrange
        var checks = new JsonObject
        {
            ["dbs"] = new JsonObject { ["status"] = "complete" },
            ["la_check"] = new JsonObject { ["status"] = (JsonNode?)null },
            ["ofsted"] = new JsonObject { ["status"] = "complete" }
        };

        // Act
        var result = ApplicationService.CalculateProgress(checks);

        // Assert
        Assert.Equal(67, result); // 2 out of 3 complete
    }

    [Fact]
    public void CalculateProgress_RealWorldScenario_11Checks()
    {
        // Arrange - Typical application with 11 checks
        var checks = new JsonObject
        {
            ["dbs"] = new JsonObject { ["status"] = "complete" },
            ["dbs_update"] = new JsonObject { ["status"] = "complete" },
            ["la_check"] = new JsonObject { ["status"] = "complete" },
            ["ofsted"] = new JsonObject { ["status"] = "pending" },
            ["gp_health"] = new JsonObject { ["status"] = "not-started" },
            ["ref_1"] = new JsonObject { ["status"] = "complete" },
            ["ref_2"] = new JsonObject { ["status"] = "complete" },
            ["first_aid"] = new JsonObject { ["status"] = "complete" },
            ["safeguarding"] = new JsonObject { ["status"] = "complete" },
            ["food_hygiene"] = new JsonObject { ["status"] = "complete" },
            ["insurance"] = new JsonObject { ["status"] = "not-started" }
        };

        // Act
        var result = ApplicationService.CalculateProgress(checks);

        // Assert
        // 8 out of 11 = 72.727... rounds to 73
        Assert.Equal(73, result);
    }
}
