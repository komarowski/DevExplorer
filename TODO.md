# DevExplorer — Development Plan & Architecture Notes

This document contains **all implementation details** for DevExplorer:
- project structure
- architectural decisions
- design patterns
- step-by-step development plan

This file is intentionally verbose.
`README.md` stays short.

## 1. High-level architecture decision

### Project split (minimal but clean)

```bash
├── DevExplorer.Domain ← domain types + contracts + base classes
├── DevExplorer.Infrastructure ← concrete implementations + adapters
│   └── Private/ ← secrets not committed to GitHub
├── DevExplorer.Api ← ASP.NET Core API
├── DevExplorer.Angular ← Angular frontend
└── DevExplorer.Tests ← unit tests
```

**Why Domain and Infrastructure are separate**
- cleaner dependency flow: Domain has no dependencies on Infrastructure
- Infrastructure depends on Domain (contracts live in Domain)
- base classes and abstractions live in Domain (e.g., `TextParserBase`)
- concrete implementations and secrets live in Infrastructure
- separation is physical (different projects) + logical (folders)
- easier to maintain and reason about dependencies

## 2. DevExplorer.Domain — internal structure

Pure domain types and contracts (no implementations of external adapters).

```bash
DevExplorer.Domain                         # Domain library: models + contracts + base classes (no external IO)
├── Models                                 # Data structures used across the system
│   ├── LogEvent.cs                        # Parsed/structured event (timestamp, level, message, correlationId, blocks)
│   ├── SearchHit.cs                       # Search result item (event + highlights + source info)
│   └── IndexStatus.cs                     # Index job state/progress (files processed, lines read, elapsed, errors)
│
├── Abstractions                           # Base classes and shared patterns for parsers/sources
│   ├── TextParserBase.cs                  # Base parser with shared helpers + virtual hooks (OOP practice)
│   └── LogSourceBase.cs                   # Base source with common async patterns (optional; add when needed)
│
├── Contracts                              # Interfaces ("ports") used by infrastructure
│   ├── ILogParser.cs                      # Converts LogRecord → LogEvent (parsing strategy)
│   ├── ILogIndexStore.cs                  # Stores and queries indexed events (in-memory/sqlite later)
│   ├── IEnrichmentProvider.cs             # Enriches hits with extra info (DB/API lookups)
│   ├── ILogPathPolicy.cs                  # Resolves “environment/source” → real log folders/patterns
│   ├── IDbQueryCatalog.cs                 # Maps logical query names → real SQL/SP names + parameters
│   └── IApiEndpointCatalog.cs             # Maps logical operations → real endpoints + auth requirements

DevExplorer.Infrastructure                 # Implementations (adapters) for IO + pipeline mechanics
│   ├── Parsers                            # Concrete parser implementations (strategies)
│   │   ├── StandardTextLogParser.cs       # Parser for plain text logs (implements TextParserBase)
│   │   └── JsonLogParser.cs               # Parser for JSON-per-line logs (add when needed)
│   │
│   ├── Indexing                           # Indexing orchestration and TPL pipeline
│   │   ├── IndexingPipeline.cs            # Channel-based producer/consumer pipeline (bounded concurrency)
│   │   ├── IndexJob.cs                    # Represents a single indexing run (config + token + lifecycle)
│   │   └── IndexProgressTracker.cs        # Thread-safe counters + progress snapshots for UI/API
│   │
│   ├── Storage                            # Storage implementations for indexed data
│   │   ├── InMemoryIndexStore.cs          # Thread-safe store for MVP (fast dev, no persistence)
│   │   └── SqliteIndexStore.cs            # Persistent store for big logs + fast search (later)
│   │
│   └── Private                            # ⚠️ Secrets — NOT committed to GitHub
│       ├── .gitignore                     # Ignore entire Private/ folder
│       ├── LogPathPolicy.cs               # Real folder paths (secrets)
│       ├── DbQueryCatalog.cs              # Real stored procedure names (secrets)
│       ├── ApiEndpointCatalog.cs          # Internal API endpoints + auth (secrets)
│       └── appsettings.secrets.json       # API keys, connection strings (secrets)
```

**Dependency rules**

- API depends on Domain and Infrastructure. Domain and Infrastructure never depend on API.
- Domain must not reference Infrastructure (Interfaces live in Domain, implementations live in Infrastructure).
- Infrastructure depends on Domain (contracts + base classes).
- Tests can reference Domain and Infrastructure, but prefer targeting Domain first.
- Sensitive implementations live in Infrastructure/Private/ (never committed).

**Rules for both projects**
- Domain: no async, no cancellation tokens in interfaces (only in implementations)
- Infrastructure: async everywhere, cancellation tokens everywhere
- Both: thread-safe where applicable
- TextParserBase lives in Domain/Abstractions (not in Infrastructure)


## 4. Design patterns used (intentionally minimal)

### Patterns to use
- Strategy → log parsers
- Pipeline → indexing (Channel)
- Adapter → file system / DB / API
- Factory → log source selection (later)
- Decorator → caching / retries (only if needed)

### Patterns to avoid early
- generic repositories
- base service classes
- Mediator / CQRS frameworks
- specification pattern
- unnecessary inheritance

**Rule:** add a pattern only when a second concrete use case appears.


## 5. Testing strategy

### Unit tests (`DevExplorer.Tests`)
Focus:
- parsing rules
- correlationId extraction
- context window logic
- domain invariants

No IO. No threading unless explicitly testing concurrency primitives.

### Integration tests (added later)
Focus:
- indexing real temp folders
- cancellation behavior
- concurrent indexing correctness
- EF Core behavior (SQLite or Testcontainers)


## 6. API layer (`DevExplorer.Api`)

### Responsibilities
- host background jobs
- expose endpoints
- manage auth, routing, middleware
- translate HTTP → core calls

### Planned endpoints
- `POST /api/v1/index/start`
- `GET  /api/v1/index/status/{jobId}`
- `POST /api/v1/index/cancel/{jobId}`
- `GET  /api/v1/search`
- `GET  /api/v1/context`

### ASP.NET Core topics to practice
- middleware pipeline
- endpoint filters
- routing & versioning
- JWT authentication & authorization


## 7. Frontend (`DevExplorer.Angular` — Angular)

### Scope (keep minimal)
- start indexing job
- show progress + cancel
- search logs
- view log details + context
- basic loading / error states

No complex state management initially.


## 8. Private folder (`DevExplorer.Infrastructure/Private`) — secrets

**Not committed to GitHub.**

This folder is where sensitive configuration goes:
- Real folder paths and environment-specific settings
- Real database stored procedure names or SQL queries
- Internal API endpoints and authentication credentials
- API keys and connection strings

Create a `.gitignore` file in `DevExplorer.Infrastructure/Private/` to prevent accidental commits:

```
# DevExplorer.Infrastructure/Private/.gitignore
*
!.gitignore
```

Implementations in this folder (implementing contracts from Domain):
- `LogPathPolicy.cs` — maps logical paths to real file locations
- `DbQueryCatalog.cs` — maps logical operation names to real SQL/SP names
- `ApiEndpointCatalog.cs` — maps logical operations to real API endpoints + auth requirements
- `appsettings.secrets.json` — connection strings, API keys, etc.

**Load at runtime:**
```csharp
// In DevExplorer.Api/Program.cs
var secretsConfig = new ConfigurationBuilder()
    .AddJsonFile("Infrastructure/Private/appsettings.secrets.json", 
        optional: true, reloadOnChange: true)
    .Build();
```

Public repo ships only demo/stub implementations in Infrastructure/ root.


## 9. Development plan (step by step)

### Phase 1 — Core + Domain logic
- [ ] Define models and interfaces
- [ ] Implement basic parser
- [ ] Write unit tests for parsing

### Phase 2 — Indexing pipeline
- [ ] Implement async file reader
- [ ] Channel-based pipeline
- [ ] In-memory index store
- [ ] Cancellation support

### Phase 3 — Search & context
- [ ] Search by text
- [ ] CorrelationId filtering
- [ ] Context window endpoint

### Phase 4 — API
- [ ] Minimal API endpoints
- [ ] Background jobs
- [ ] Middleware (correlation, exceptions)

### Phase 5 — Frontend
- [ ] Angular UI (minimal)
- [ ] Index + search workflow

### Phase 6 — Enrichment
- [ ] DB lookup adapter
- [ ] API lookup adapter
- [ ] Optional caching decorator

### Phase 7 — Auth & polish
- [ ] JWT auth
- [ ] Roles & policies
- [ ] SQLite index storage (optional)


## 10. Post-project mandatory step

After DevExplorer is complete:

Write short, concise articles:
- C# fundamentals
- Async & TPL
- ASP.NET Core & JWT
- Angular as API client

These articles are part of the learning goal and interview preparation.

## 11. Guiding principles

- clarity over cleverness
- async correctness over micro-optimizations
- real usefulness over architectural purity
- explainable decisions > “best practices”
