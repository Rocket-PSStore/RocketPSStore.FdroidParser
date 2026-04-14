using RocketPSStore.FdroidParser.Models;
using Xunit;

namespace RocketPSStore.FdroidParser.Tests;

public class ParserTests
{
    /// <summary>
    /// Verifies that DisplayName correctly falls back when specific locales are missing.
    /// </summary>
    [Fact]
    public void DisplayName_ShouldPrioritizeEnglish()
    {
        // Arrange
        var app = new FdroidApp
        {
            PackageName = "com.rocket.app",
            Name = new Dictionary<string, string>
            {
                { "fr", "Application Fusée" },
                { "en-US", "Rocket App" }
            }
        };

        // Act & Assert
        Assert.Equal("Rocket App", app.DisplayName);
    }

    /// <summary>
    /// Verifies that DisplayName falls back to PackageName if no names are found.
    /// </summary>
    [Fact]
    public void DisplayName_ShouldFallbackToPackageName_WhenNamesEmpty()
    {
        // Arrange
        var app = new FdroidApp { PackageName = "com.rocket.invisible" };

        // Act & Assert
        Assert.Equal("com.rocket.invisible", app.DisplayName);
    }

    /// <summary>
    /// Verifies that the Icon URL is constructed correctly using the F-Droid standard.
    /// </summary>
    [Fact]
    public void GetIconUrl_ShouldFormatCorrectly()
    {
        // Arrange
        var app = new FdroidApp { PackageName = "org.fdroid.fdroid" };
        string baseUrl = "https://f-droid.org/repo";

        // Act
        var url = app.GetIconUrl(baseUrl);

        // Assert
        Assert.Equal("https://f-droid.org/repo/org.fdroid.fdroid/en-US/icon.png", url);
    }

    /// <summary>
    /// Verifies that the FdroidClient can be initialized (tests the Constructor logic).
    /// </summary>
    [Fact]
    public void FdroidClient_ShouldInitializeWithoutError()
    {
        // Act
        using var client = new FdroidClient();
        
        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public async Task FdroidClient_LiveIntegration_ShouldReturnFirstTenApps()
    {
        // Arrange
        using var client = new FdroidClient();
        string indexUrl = "https://f-droid.org/repo/index-v2.json";
        int count = 0;

        // Act
        await foreach (var app in client.StreamAppsAsync(indexUrl))
        {
            count++;
            
            // Assert: Ensure we are actually getting data
            Assert.NotNull(app.PackageName);
            Assert.NotEmpty(app.PackageName);

            // We only want the first 10 to keep the test fast
            if (count >= 10) break;
        }

        // Assert: Verify we actually hit our limit
        Assert.Equal(10, count);
    }
}