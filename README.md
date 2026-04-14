# RocketPSStore.FdroidParser
The official .NET implementation of the Rocket PSStore parsing engine. A high-performance, strongly-typed library for handling modern F-Droid ``index-v2.json`` repositories in C#. Built to bring Rocket-speed repository management to the .NET ecosystem.

## Package template
This repository now includes a minimal NuGet package template for `RocketPSStore.FdroidParser`.

### Build and pack locally
1. Open a terminal in the repository root.
2. Run:
   ```bash
   dotnet restore src/RocketPSStore.FdroidParser/RocketPSStore.FdroidParser.csproj
   dotnet build src/RocketPSStore.FdroidParser/RocketPSStore.FdroidParser.csproj --configuration Release
   dotnet pack src/RocketPSStore.FdroidParser/RocketPSStore.FdroidParser.csproj --configuration Release --output ./artifacts
   ```
3. The package file will be created in `./artifacts`.

### GitHub Actions
A workflow is included at `.github/workflows/dotnet-pack.yml` that restores, builds, and packs the project on every push or pull request to `main`.

### Publish to GitHub Packages or NuGet.org
To publish automatically, extend the workflow with `dotnet nuget push` and add the appropriate repository URL and secret for `NUGET_API_KEY`.

### Example usage
```csharp
using RocketPSStore.FdroidParser;

using var client = new FdroidClient();
var indexUrl = "https://f-droid.org/repo/index-v2.json";

await foreach (var app in client.StreamAppsAsync(indexUrl))
{
    Console.WriteLine($"Found: {app.DisplayName}");
    
    // Stop whenever you want to save data
    if (app.PackageName == "org.fdroid.fdroid") break;
}
```
