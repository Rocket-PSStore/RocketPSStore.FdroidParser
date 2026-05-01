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

    [Fact]
    public void ParseAppMetadataYaml_ShouldPopulateAppFromYaml()
    {
        // Arrange
        var yaml = @"name:
  en: Rocket App
  fr: Application Fusée
summary:
  en: Fast and lightweight.
description:
  en: This app rockets your productivity.
license: GPL-3.0-or-later
website: https://rocket.example.com
SourceCode: https://git.example.com/rocket
IssueTracker: https://issues.example.com/rocket
AuthorName: F-Droid
AuthorEmail: team@f-droid.org
Translation: https://hosted.weblate.org/projects/f-droid/f-droid
Changelog: https://gitlab.com/fdroid/fdroidclient/-/blob/HEAD/CHANGELOG.md
Donate: https://f-droid.org/donate
Liberapay: F-Droid-Data
OpenCollective: F-Droid-Euro
Bitcoin: bc1qd8few44yaxc3wv5ceeedhdszl238qkvu50rj4v
RepoType: git
Repo: https://gitlab.com/fdroid/fdroidclient.git
MaintainerNotes: |
  This is maintained in tight conjunction with fdroidclient releases.
ArchivePolicy: 5
AutoUpdateMode: None
UpdateCheckMode: Static
CurrentVersion: 1.23.2
CurrentVersionCode: 1023052
builds:
  - versionName: '0.17'
    versionCode: 8
    commit: c626ce5f6d3e10ae15942f01ff028be310cc695a
categories:
  - Productivity
  - Utilities
";

        // Act
        var app = FdroidParser.ParseAppMetadataYaml(yaml, "com.rocket.app");

        // Assert
        Assert.Equal("com.rocket.app", app.PackageName);
        Assert.Equal("Rocket App", app.Name["en"]);
        Assert.Equal("Application Fusée", app.Name["fr"]);
        Assert.Equal("Fast and lightweight.", app.Summary);
        Assert.Equal("This app rockets your productivity.", app.Description);
        Assert.Equal("GPL-3.0-or-later", app.License);
        Assert.Equal("https://rocket.example.com", app.Website);
        Assert.Equal("https://git.example.com/rocket", app.SourceCode);
        Assert.Equal("https://issues.example.com/rocket", app.IssueTracker);
        Assert.Equal("F-Droid", app.AuthorName);
        Assert.Equal("team@f-droid.org", app.AuthorEmail);
        Assert.Equal("https://hosted.weblate.org/projects/f-droid/f-droid", app.Translation);
        Assert.Equal("https://gitlab.com/fdroid/fdroidclient/-/blob/HEAD/CHANGELOG.md", app.Changelog);
        Assert.Equal("https://f-droid.org/donate", app.Donate);
        Assert.Equal("F-Droid-Data", app.Liberapay);
        Assert.Equal("F-Droid-Euro", app.OpenCollective);
        Assert.Equal("bc1qd8few44yaxc3wv5ceeedhdszl238qkvu50rj4v", app.Bitcoin);
        Assert.Equal("git", app.RepoType);
        Assert.Equal("https://gitlab.com/fdroid/fdroidclient.git", app.Repo);
        Assert.Equal("This is maintained in tight conjunction with fdroidclient releases.", app.MaintainerNotes.Trim());
        Assert.Equal(5, app.ArchivePolicy);
        Assert.Equal("None", app.AutoUpdateMode);
        Assert.Equal("Static", app.UpdateCheckMode);
        Assert.Equal("1.23.2", app.CurrentVersion);
        Assert.Equal(1023052, app.CurrentVersionCode);
        Assert.Single(app.Builds);
        Assert.Equal("c626ce5f6d3e10ae15942f01ff028be310cc695a", app.Builds[0]["commit"]);
        Assert.Contains("Productivity", app.Categories);
        Assert.Contains("Utilities", app.Categories);
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