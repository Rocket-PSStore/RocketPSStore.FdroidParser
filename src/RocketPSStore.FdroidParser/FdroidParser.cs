namespace RocketPSStore.FdroidParser;

/// <summary>
/// Provides utility methods for identifying and parsing F-Droid repository indexes.
/// </summary>
public static class FdroidParser
{
    /// <summary>
    /// Validates if the provided string content follows the basic structure of an F-Droid JSON index.
    /// </summary>
    /// <param name="content">The raw string content to check.</param>
    /// <returns>True if the content starts with a JSON object brace; otherwise, false.</returns>
    public static bool IsIndexJson(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        return content.TrimStart().StartsWith('{');
    }
}