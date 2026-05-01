using System;
using System.Collections.Generic;
using System.Linq;
using RocketPSStore.FdroidParser.Models;
using YamlDotNet.Serialization;

#nullable enable

namespace RocketPSStore.FdroidParser;

/// <summary>
/// Provides utility methods for identifying and parsing F-Droid repository indexes and metadata.
/// </summary>
public static class FdroidParser
{
    private static readonly HashSet<string> KnownMetadataKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "name",
        "summary",
        "description",
        "license",
        "website",
        "source",
        "source-code",
        "source_code",
        "sourcecode",
        "issue-tracker",
        "issue_tracker",
        "issuetracker",
        "authorname",
        "author-name",
        "author_name",
        "authoremail",
        "author-email",
        "author_email",
        "email",
        "translation",
        "changelog",
        "donate",
        "liberapay",
        "opencollective",
        "bitcoin",
        "repotype",
        "repo-type",
        "repo_type",
        "repo",
        "autoname",
        "auto-name",
        "auto_name",
        "maintainernotes",
        "maintainer-notes",
        "maintainer_notes",
        "archivepolicy",
        "archive-policy",
        "archive_policy",
        "autoupdatemode",
        "auto-update-mode",
        "auto_update_mode",
        "updatecheckmode",
        "update-check-mode",
        "update_check_mode",
        "currentversion",
        "current-version",
        "current_version",
        "currentversioncode",
        "current-version-code",
        "current_version_code",
        "builds",
        "categories"
    };

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

    /// <summary>
    /// Parses F-Droid metadata YAML into an <see cref="FdroidApp"/> instance.
    /// </summary>
    /// <param name="yamlContent">The YAML content from the F-Droid metadata file.</param>
    /// <param name="packageName">The package name of the application.</param>
    /// <returns>A populated <see cref="FdroidApp"/> instance.</returns>
    public static FdroidApp ParseAppMetadataYaml(string yamlContent, string packageName)
    {
        if (string.IsNullOrWhiteSpace(packageName))
        {
            throw new ArgumentException("Package name is required.", nameof(packageName));
        }

        if (string.IsNullOrWhiteSpace(yamlContent))
        {
            throw new ArgumentException("YAML content cannot be null or empty.", nameof(yamlContent));
        }

        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();

        var raw = deserializer.Deserialize<Dictionary<object, object>>(yamlContent) ?? new Dictionary<object, object>();
        var app = new FdroidApp
        {
            PackageName = packageName,
            Name = ParseLocalizedStringDictionary(raw, "name"),
            Summary = ParseLocalizedString(raw, "summary") ?? string.Empty,
            Description = ParseLocalizedString(raw, "description") ?? string.Empty,
            License = GetStringValue(raw, "license"),
            Website = GetStringValue(raw, "website"),
            SourceCode = GetStringValue(raw, "source", "source-code", "source_code", "sourcecode"),
            IssueTracker = GetStringValue(raw, "issue-tracker", "issue_tracker", "issuetracker"),
            AuthorName = GetStringValue(raw, "authorname", "author-name", "author_name"),
            AuthorEmail = GetStringValue(raw, "authoremail", "author-email", "author_email", "email"),
            Translation = GetStringValue(raw, "translation"),
            Changelog = GetStringValue(raw, "changelog"),
            Donate = GetStringValue(raw, "donate"),
            Liberapay = GetStringValue(raw, "liberapay"),
            OpenCollective = GetStringValue(raw, "opencollective"),
            Bitcoin = GetStringValue(raw, "bitcoin"),
            RepoType = GetStringValue(raw, "repotype", "repo-type", "repo_type"),
            Repo = GetStringValue(raw, "repo"),
            AutoName = GetStringValue(raw, "autoname", "auto-name", "auto_name"),
            MaintainerNotes = GetStringValue(raw, "maintainernotes", "maintainer-notes", "maintainer_notes"),
            ArchivePolicy = ParseIntValue(raw, "archivepolicy", "archive-policy", "archive_policy"),
            AutoUpdateMode = GetStringValue(raw, "autoupdatemode", "auto-update-mode", "auto_update_mode"),
            UpdateCheckMode = GetStringValue(raw, "updatecheckmode", "update-check-mode", "update_check_mode"),
            CurrentVersion = GetStringValue(raw, "currentversion", "current-version", "current_version"),
            CurrentVersionCode = ParseIntValue(raw, "currentversioncode", "current-version-code", "current_version_code"),
            Builds = ParseDictionaryList(raw, "builds"),
            Categories = ParseStringList(raw, "categories")
        };

        app.AdditionalMetadata = raw
            .Where(entry => entry.Key is string key && !KnownMetadataKeys.Contains(key))
            .ToDictionary(entry => entry.Key!.ToString()!, entry => entry.Value, StringComparer.OrdinalIgnoreCase);

        return app;
    }

    private static Dictionary<string, string> ParseLocalizedStringDictionary(Dictionary<object, object> raw, string key)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var value = GetValue(raw, key);

        if (value is string text)
        {
            result["en"] = text;
            return result;
        }

        if (value is Dictionary<object, object> map)
        {
            foreach (var kvp in map)
            {
                if (kvp.Key is string locale && kvp.Value is string localizedText)
                {
                    result[locale] = localizedText;
                }
            }

            return result;
        }

        return result;
    }

    private static string? ParseLocalizedString(Dictionary<object, object> raw, string key)
    {
        var value = GetValue(raw, key);

        if (value is string text)
        {
            return text;
        }

        if (value is Dictionary<object, object> map)
        {
            if (map.TryGetValue("en-US", out var enUs) && enUs is string enUsText)
            {
                return enUsText;
            }

            if (map.TryGetValue("en", out var en) && en is string enText)
            {
                return enText;
            }

            return map.Values.OfType<string>().FirstOrDefault();
        }

        return null;
    }

    private static List<string> ParseStringList(Dictionary<object, object> raw, string key)
    {
        var result = new List<string>();
        var value = GetValue(raw, key);

        if (value is IEnumerable<object> list)
        {
            foreach (var item in list)
            {
                if (item is string s)
                {
                    result.Add(s);
                }
            }
        }
        else if (value is string singleValue)
        {
            result.Add(singleValue);
        }

        return result;
    }

    private static string? GetStringValue(Dictionary<object, object> raw, params string[] keys)
    {
        var value = GetValue(raw, keys);
        return value as string;
    }

    private static int? ParseIntValue(Dictionary<object, object> raw, params string[] keys)
    {
        var value = GetValue(raw, keys);
        switch (value)
        {
            case int i:
                return i;
            case long l when l <= int.MaxValue && l >= int.MinValue:
                return (int)l;
            case string s when int.TryParse(s, out var parsed):
                return parsed;
            default:
                return null;
        }
    }

    private static List<Dictionary<string, object>> ParseDictionaryList(Dictionary<object, object> raw, params string[] keys)
    {
        var result = new List<Dictionary<string, object>>();
        var value = GetValue(raw, keys);

        if (value is IEnumerable<object> list)
        {
            foreach (var item in list)
            {
                if (item is Dictionary<object, object> map)
                {
                    var mapped = map
                        .Where(kvp => kvp.Key is string)
                        .ToDictionary(kvp => kvp.Key!.ToString()!, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);

                    result.Add(mapped);
                }
            }
        }

        return result;
    }

    private static object? GetValue(Dictionary<object, object> raw, params string[] keys)
    {
        var lookup = BuildCaseInsensitiveLookup(raw);
        return GetValue(lookup, keys);
    }

    private static object? GetValue(Dictionary<string, object> lookup, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (lookup.TryGetValue(key, out var value))
            {
                return value;
            }
        }

        return null;
    }

    private static Dictionary<string, object> BuildCaseInsensitiveLookup(Dictionary<object, object> raw)
    {
        var lookup = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in raw)
        {
            if (entry.Key is string key && !lookup.ContainsKey(key))
            {
                lookup[key] = entry.Value;
            }
        }

        return lookup;
    }
}
