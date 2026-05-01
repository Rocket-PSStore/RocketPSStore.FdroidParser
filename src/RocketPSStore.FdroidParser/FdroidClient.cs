using System;
using System.Collections.Generic;
using System.IO.Compression;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="FdroidClient"/> class.
    /// </summary>
    public FdroidClient()
    {
        // Use a handler that supports automatic decompression
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        };
        _httpClient = new HttpClient(handler);
    }

    /// <summary>
    /// Fetches the index stream and parses it without loading the full file into memory.
    /// </summary>
    /// <param name="url">The F-Droid index URL.</param>
    /// <returns>An async enumerable of applications.</returns>
    public async IAsyncEnumerable<FdroidApp> StreamAppsAsync(string url)
    {
        using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        
        // System.Text.Json.Nodes or JsonSerializer.DeserializeAsyncEnumerable can be used here
        using var jsonDoc = await JsonDocument.ParseAsync(stream);
        
        if (jsonDoc.RootElement.TryGetProperty("packages", out var packages))
        {
            foreach (var package in packages.EnumerateObject())
            {
                var app = new FdroidApp
                {
                    PackageName = package.Name,
                    // Map your specific fields here
                    Summary = package.Value.TryGetProperty("summary", out var s) ? s.GetString() ?? "" : ""
                };
                yield return app;
            }
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

        using var response = await _httpClient.GetAsync(metadataUrl);
        response.EnsureSuccessStatusCode();

        var yaml = await response.Content.ReadAsStringAsync();
        return FdroidParser.ParseAppMetadataYaml(yaml, packageName);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}