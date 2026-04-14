# 🚀 RocketPSStore.FdroidParser

[![NuGet version](https://img.shields.io/nuget/v/RocketPSStore.FdroidParser.svg)](https://www.nuget.org/packages/RocketPSStore.FdroidParser/)
[![Build Status](https://github.com/Rocket-PSStore/RocketPSStore.FdroidParser/actions/workflows/dotnet-pack.yml/badge.svg)](https://github.com/Rocket-PSStore/RocketPSStore.FdroidParser/actions)
[![License: LGPL-2.1](https://img.shields.io/badge/License-LGPL_2.1-blue.svg)](https://opensource.org/licenses/LGPL-2.1)

**The high-performance core of the Rocket PSStore ecosystem.**

`RocketPSStore.FdroidParser` is a modern, strongly-typed .NET library specifically engineered to handle massive F-Droid `index-v2.json` repositories. Designed for speed and memory efficiency, it brings "Rocket-class" repository management to the C# and .NET ecosystem.

---

## ✨ Features

* **⚡ Zero-Footprint Streaming:** Processes the 30MB+ F-Droid index entry-by-entry. No more `OutOfMemoryException` on mobile devices.
* **📦 Native GZIP Support:** Automatic decompression handling using modern `HttpClient` protocols.
* **🌍 Intelligent Localization:** Built-in logic to prioritize your preferred language (e.g., `en-US`) with smart fallbacks.
* **🔗 Async-First:** Fully utilizes `IAsyncEnumerable` for non-blocking UI updates.
* **🛡️ Battle-Tested:** Integrated unit tests ensure data integrity across all .NET 8+ runtimes.

---

## 🚀 Getting Started

### Installation

Install via the NuGet Package Manager:

```bash
dotnet add package RocketPSStore.FdroidParser
````

### Basic Usage

Processing a repository index is as simple as a few lines of code. The parser streams data directly from the source, allowing you to stop whenever you find what you need.

```csharp
using RocketPSStore.FdroidParser;

// Initialize the Rocket Client
using var client = new FdroidClient();
const string repoUrl = "[https://f-droid.org/repo/index-v2.json](https://f-droid.org/repo/index-v2.json)";

// Stream apps one by one to keep memory usage low
await foreach (var app in client.StreamAppsAsync(repoUrl))
{
    Console.WriteLine($"Found App: {app.DisplayName}");
    
    // Efficiently exit the stream early
    if (app.PackageName == "org.fdroid.fdroid") 
    {
        Console.WriteLine("Found the official F-Droid app!");
        break; 
    }
}
```

-----

## 🛠 Development & Contribution

We welcome contributions to the Rocket engine\! Whether it's fixing a bug or suggesting a new feature, your help makes the Rocket fly higher.

### Local Setup

1.  Clone the repository.
2.  Build the project:
    ```bash
    dotnet build --configuration Release
    ```
3.  Run the test suite:
    ```bash
    dotnet test
    ```

### Roadmap

  - [x] High-performance GZIP streaming.
  - [x] Initial NuGet Release.
  - [ ] .NET MAUI Integration Samples.
  - [ ] Multi-repository support (v3 index format).

-----

## 📄 License

This project is licensed under the **LGPL-2.1 License** - see the [LICENSE](LICENSE) file for details.