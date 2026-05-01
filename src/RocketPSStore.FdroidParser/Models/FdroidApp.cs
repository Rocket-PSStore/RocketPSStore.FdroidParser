using System.Collections.Generic;

namespace RocketPSStore.FdroidParser.Models;

/// <summary>
/// Represents an application in the F-Droid repository with its associated metadata.
/// </summary>
public class FdroidApp
{
    /// <summary>
    /// Gets or sets the unique Android package identifier (e.g., "com.example.app").
    /// </summary>
    public string PackageName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a dictionary mapping language codes to the app's localized names.
    /// </summary>
    /// <remarks>
    /// Keys are typically BCP-47 language tags like "en-US" or "fr".
    /// </remarks>
    public Dictionary<string, string> Name { get; set; } = new();
    
    /// <summary>
    /// Gets or sets a short summary description of the application.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full description of the application.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the license identifier for the application.
    /// </summary>
    public string License { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the website URL associated with the application.
    /// </summary>
    public string Website { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source code URL for the application.
    /// </summary>
    public string SourceCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the issue tracker URL for the application.
    /// </summary>
    public string IssueTracker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author's name for the application.
    /// </summary>
    public string AuthorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author's email address for the application.
    /// </summary>
    public string AuthorEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the translation page URL for the application.
    /// </summary>
    public string Translation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the changelog URL for the application.
    /// </summary>
    public string Changelog { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the donation URL for the application.
    /// </summary>
    public string Donate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Liberapay project name for the application.
    /// </summary>
    public string Liberapay { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OpenCollective project name for the application.
    /// </summary>
    public string OpenCollective { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Bitcoin address for the application.
    /// </summary>
    public string Bitcoin { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of repository used for the application source.
    /// </summary>
    public string RepoType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the repository URL or identifier for the application.
    /// </summary>
    public string Repo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the auto-generated display name for the application.
    /// </summary>
    public string AutoName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maintainer notes from the metadata file.
    /// </summary>
    public string MaintainerNotes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the archive policy value from the metadata file.
    /// </summary>
    public int? ArchivePolicy { get; set; }

    /// <summary>
    /// Gets or sets the auto-update mode for the application.
    /// </summary>
    public string AutoUpdateMode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the update check mode for the application.
    /// </summary>
    public string UpdateCheckMode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current version of the application.
    /// </summary>
    public string CurrentVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current version code of the application.
    /// </summary>
    public int? CurrentVersionCode { get; set; }

    /// <summary>
    /// Gets or sets the list of build metadata entries for the application.
    /// </summary>
    public List<Dictionary<string, object>> Builds { get; set; } = new();

    /// <summary>
    /// Gets or sets any additional top-level metadata keys present in the YAML file.
    /// </summary>
    public Dictionary<string, object> AdditionalMetadata { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of categories assigned to the application.
    /// </summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>
    /// Gets the most appropriate name to display, prioritizing English (en-US or en) 
    /// before falling back to the package name.
    /// </summary>
    public string DisplayName => Name.ContainsKey("en-US") ? Name["en-US"] : 
                                 Name.ContainsKey("en") ? Name["en"] : 
                                 PackageName;

    /// <summary>
    /// Generates a standard URL for the application's icon based on the repository's base address.
    /// </summary>
    /// <param name="repoBaseUrl">The base URL of the F-Droid repository (e.g., "https://f-droid.org/repo").</param>
    /// <returns>A fully qualified URL to the en-US icon resource.</returns>
    public string GetIconUrl(string repoBaseUrl) => $"{repoBaseUrl}/{PackageName}/en-US/icon.png";
}