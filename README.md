# DevExplorer

DevExplorer is a **local developer tool** for exploring technical data from multiple sources (logs, database, APIs)

Its goals are twofold:

1. Be genuinely useful for debugging and investigation tasks
2. Serve as a practical learning project for modern .NET backend development

## Project Structure

```
DevExplorer/
├── DevExplorer.Domain/                    # Core abstractions and models
│
├── DevExplorer.Infrastructure/            # Implementations
│   ├── Data/                              # Data access
│   ├── Providers/                         # ProjectProvider with reflection discovery
│   ├── Projects/                          # Public project implementations
│   ├── PrivateProjects/                   # Git-ignored project implementations
│   └── Services/                          # ProjectSettingsService
│
├── DevExplorer.Api/                       # ASP.NET Core minimal API
├── DevExplorer.React/                     # React + TypeScript frontend
└── DevExplorer.Tests/                     # xUnit tests
```

## Plugin Architecture Design

**Why This Approach:**
- Each project has different data sources (logs, databases, APIs)
- Base classes (`ProjectBase`, `TextParserBase`) reduce boilerplate
- Environment-aware settings via `IProjectSettingsService`
- Git-friendly: PrivateProjects/ folder is ignored

**How It Works:**
1. Projects inherit from `ProjectBase` and use `[Project]` attribute
2. `ProjectProvider` scans assemblies and caches discovered projects
3. `IProjectSettingsService` provides environment-specific configuration
4. Each project can have custom `IProjectLogParser` for log formats