# DevExplorer — Development Plan

Plugin architecture for multi-project log exploration system.

## Architecture Overview

**Plugin Pattern:** Each project implements minimal IProject interface (Name property + GetLogsAsync method). ProjectProvider uses static reflection-based discovery with thread-safe caching.

## Project Structure

```
DevExplorer/
├── DevExplorer.Domain/                    # Domain models + contracts (no implementations)
│   ├── Models/
│   │   └── LogEvent.cs                    # Timestamp, Level, Message, FilePath, CorrelationId
│   ├── Abstractions/
│   │   └── TextParserBase.cs              # Multi-line buffering parser base class
│   └── Contracts/
│       ├── IProject.cs                    # Name property + GetLogsAsync method
│       ├── IProjectRegistry.cs            # Registry interface
│       ├── IProjectLogParser.cs           # Parser interface
│       └── IProjectLogSearcher.cs         # Searcher interface
│
├── DevExplorer.Infrastructure/            # Implementations (adapters)
│   ├── Providers/
│   │   └── ProjectProvider.cs             # Static registry with reflection discovery
│   ├── Projects/
│   │   └── Demo/                          # Public example project
│   └── PrivateProjects/                   # Git-ignored secrets
│
├── DevExplorer.Api/                       # ASP.NET Core API
│
├── DevExplorer.React/                     # React frontend
│
└── DevExplorer.Tests/                     # xUnit tests
```

## Plugin Architecture Design

**Why This Approach:**
- Each project has fundamentally different log capabilities
- Registry Pattern + simple discovery eliminates boilerplate factories
- Git-friendly: PrivateProjects/ folder keeps secrets out of repository
- Extensible: add IProject class → automatic discovery via reflection

**How It Works:**
1. Each project implements `IProject` (Name + GetLogsAsync)
2. `ProjectProvider.Initialize()` scans assemblies for IProject implementations
3. Thread-safe cache stores discovered projects by name
4. API calls ProjectProvider to dispatch to correct project

**Example:** AdministrationProject searches error logs by date, applies filters, returns List<LogEvent>

## Development Progress

| Phase | Status | Tasks |
|-------|--------|-------|
| Phase 1: Core + Domain Logic | ✅ COMPLETED | Models, contracts, TextParserBase, AdministrationProject, ProjectProvider |
| Phase 1.5: API + React Frontend | ✅ COMPLETED | API endpoints, React UI with sidebar/tabs, log filtering, color-coded display |
| Phase 2: Indexing Pipeline | ⏳ PENDING | Async file reader, Channel-based pipeline, in-memory store, cancellation |
| Phase 3: Search & Context | ⏳ PENDING | Text search, CorrelationId filtering, context window endpoint |
| Phase 4: API & Background Jobs | ⏳ PENDING | Index endpoints, background jobs, middleware |
| Phase 5: Advanced Features | ⏳ PENDING | More projects, enrichment, SQLite, JWT auth |

## Guiding Principles

- Clarity over cleverness
- Async correctness over micro-optimizations
- Real usefulness over architectural purity
- Explainable decisions > "best practices"
- Add patterns only when second use case appears
