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