#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RocketPSStore.FdroidParser.Models;

namespace RocketPSStore.FdroidParser;

/// <summary>
/// Provides methods to download and stream F-Droid repository data.
/// </summary>
public class FdroidClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="FdroidClient"/> class with a default HTTP client.
    /// </summary>
    public FdroidClient()
        : this(CreateDefaultHttpClient(), disposeHttpClient: true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FdroidClient"/> class with an externally provided HTTP client.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for requests.</param>
    /// <param name="disposeHttpClient">Whether to dispose the provided HTTP client when this instance is disposed.</param>
    public FdroidClient(HttpClient httpClient, bool disposeHttpClient = false)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _disposeHttpClient = disposeHttpClient;
    }

    /// <summary>
    /// Fetches the index and yields application metadata entries from the repository.
    /// </summary>
    /// <param name="url">The F-Droid index URL.</param>
    /// <returns>An async enumerable of applications.</returns>
    public async IAsyncEnumerable<FdroidApp> StreamAppsAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Index URL is required.", nameof(url));
        }

        using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        await foreach (var app in ParsePackageEntriesAsync(stream))
        {
            yield return app;
        }
    }

    /// <summary>
    /// Fetches F-Droid metadata YAML for a single application and parses it into an <see cref="FdroidApp"/>.
    /// </summary>
    /// <param name="metadataUrl">The direct URL to the metadata YAML file.</param>
    /// <param name="packageName">The package name for the application.</param>
    /// <returns>A <see cref="FdroidApp"/> populated from the metadata YAML.</returns>
    public async Task<FdroidApp> FetchAppMetadataAsync(string metadataUrl, string packageName)
    {
        if (string.IsNullOrWhiteSpace(metadataUrl))
        {
            throw new ArgumentException("Metadata URL is required.", nameof(metadataUrl));
        }

        using var response = await _httpClient.GetAsync(metadataUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        var yaml = await reader.ReadToEndAsync();

        return FdroidParser.ParseAppMetadataYaml(yaml, packageName);
    }

    private static async IAsyncEnumerable<FdroidApp> ParsePackageEntriesAsync(Stream jsonStream)
    {
        using var jsonDoc = await JsonDocument.ParseAsync(jsonStream);

        if (!jsonDoc.RootElement.TryGetProperty("packages", out var packages))
        {
            yield break;
        }

        foreach (var package in packages.EnumerateObject())
        {
            yield return new FdroidApp
            {
                PackageName = package.Name,
                Summary = GetStringProperty(package.Value, "summary") ?? string.Empty
            };
        }
    }

    private static string? GetStringProperty(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static HttpClient CreateDefaultHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        };

        return new HttpClient(handler, disposeHandler: true);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }
}
