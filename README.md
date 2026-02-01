# DevExplorer

DevExplorer is a **local developer tool** for exploring technical data from multiple sources (logs, database, APIs)

Its goals are twofold:

1. Be genuinely useful for debugging and investigation tasks
2. Serve as a practical learning project for modern .NET backend development

## Project Structure

```
DevExplorer/
├── DevExplorer.Domain/                    # Models + contracts
│
├── DevExplorer.Infrastructure/            # Implementations (adapters)
│   ├── Providers/
│   │   └── ProjectProvider.cs             # Static registry with reflection discovery
│   ├── Projects/
│   │   └── Demo/                          # Public example project
│   └── PrivateProjects/                   # Git-ignored secrets
│
├── DevExplorer.Api/                       # ASP.NET Core API
├── DevExplorer.React/                     # React SPA frontend
└── DevExplorer.Tests/                     # Unit + integration tests
```

## Plugin Architecture Design

**Why This Approach:**
- Each project has fundamentally different log capabilities
- Registry Pattern + simple discovery eliminates boilerplate factories
- Git-friendly: PrivateProjects/ folder keeps secrets out of repository
- Extensible: add IProject class → automatic discovery via reflection

**How It Works:**
1. Each project implements `IProject`
2. `ProjectProvider.Initialize()` scans assemblies for IProject implementations
3. Thread-safe cache stores discovered projects by name
4. API calls ProjectProvider to dispatch to correct project

## Development Progress

| Phase | Status | Tasks |
|-------|--------|-------|
| Phase 1: Core + Domain Logic | ✅ COMPLETED | Models, contracts, ProjectProvider |
| Phase 2: API + React Frontend | ✅ COMPLETED | API endpoints, React UI with sidebar/tabs, log filtering, color-coded display |
| Phase 3: Indexing Pipeline | ⏳ PENDING | Async file reader, Channel-based pipeline, in-memory store, cancellation |
| Phase 4: API & Background Jobs | ⏳ PENDING | Index endpoints, background jobs, middleware |
| Phase 5: Advanced Features | ⏳ PENDING | More projects, enrichment, SQLite, JWT auth |