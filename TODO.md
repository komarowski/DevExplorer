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
├── DevExplorer.Core ← domain + infrastructure
├── DevExplorer.Api ← ASP.NET Core API
├── DevExplorer.Angular ← Angular frontend
├── DevExplorer.Tests ← unit tests
└── (private) DevExplorer.Secrets
```

**Why Core merges Domain + Infrastructure**
- technical domain (logs, IO, parsing) is tightly coupled
- fewer projects = faster iteration
- separation is enforced via folders + interfaces
- can be split later if complexity grows

## 2. DevExplorer.Core — internal structure

Logical (not physical) layering:

```bash
DevExplorer.Core                           # Core library: models + contracts + implementations (no ASP.NET)
├── Domain                                 # Pure domain types and contracts (framework-agnostic)
│   ├── Models                             # Data structures used across the system
│   │   ├── LogRecord.cs                   # Raw log line + file metadata (path, line number, text)
│   │   ├── LogEvent.cs                    # Parsed/structured event (timestamp, level, message, correlationId, blocks)
│   │   ├── SearchHit.cs                   # Search result item (event + highlights + source info)
│   │   └── IndexStatus.cs                 # Index job state/progress (files processed, lines read, elapsed, errors)
│   │
│   ├── ValueObjects                       # Strongly-typed small domain values
│   │   └── CorrelationId.cs               # Typed correlation id with validation/normalization
│   │
│   └── Contracts                          # Interfaces (“ports”) used by core logic
│       ├── ILogSource.cs                  # Reads logs as async stream of LogRecord (file/db/api sources)
│       ├── ILogParser.cs                  # Converts LogRecord → LogEvent (parsing strategy)
│       ├── ILogIndexStore.cs              # Stores and queries indexed events (in-memory/sqlite later)
│       ├── IEnrichmentProvider.cs         # Enriches hits with extra info (DB/API lookups)
│       ├── ILogPathPolicy.cs              # Resolves “environment/source” → real log folders/patterns
│       ├── IDbQueryCatalog.cs             # Maps logical query names → real SQL/SP names + parameters
│       └── IApiEndpointCatalog.cs         # Maps logical operations → real endpoints + auth requirements
│
├── Infrastructure                         # Implementations (adapters) for IO + pipeline mechanics
│   ├── Parsing                            # Concrete parser implementations (strategies)
│   │   ├── LogParserBase.cs               # Base parser with shared helpers + virtual hooks (OOP practice)
│   │   ├── TextLogParser.cs               # Parser for plain text logs (most common starting point)
│   │   └── JsonLogParser.cs               # Parser for JSON-per-line logs (add when needed)
│   │
│   ├── Sources                            # Log source implementations (where records come from)
│   │   ├── FileLogSource.cs               # Reads files async from disk (IAsyncEnumerable)
│   │   └── DbLogSource.cs                 # Reads audit/log records from DB (later)
│   │
│   ├── Indexing                           # Indexing orchestration and TPL pipeline
│   │   ├── IndexingPipeline.cs            # Channel-based producer/consumer pipeline (bounded concurrency)
│   │   ├── IndexJob.cs                    # Represents a single indexing run (config + token + lifecycle)
│   │   └── IndexProgressTracker.cs        # Thread-safe counters + progress snapshots for UI/API
│   │
│   └── Storage                            # Storage implementations for indexed data
│       ├── InMemoryIndexStore.cs          # Thread-safe store for MVP (fast dev, no persistence)
│       └── SqliteIndexStore.cs            # Persistent store for big logs + fast search (later)
│
└── Common                                 # Small shared utilities (keep minimal)
    ├── Guard.cs                           # Guard clauses (null/empty/range checks, throw helpers)
    └── Result.cs                          # Lightweight result type (optional; avoid if unused)
```

**Dependency rules**

- API depends on Core. Core never depends on API.
- Domain must not reference Infrastructure (Interfaces live in Domain, implementations live in Infrastructure.).
- Infrastructure may reference Domain (and Common).
- Tests can reference Core (Domain + Infrastructure), but prefer targeting Domain first.
- Sensitive implementations live outside the public repo.


**Rules**
- no ASP.NET references here
- async everywhere
- cancellation tokens everywhere
- thread-safe storage


## 3. Design patterns used (intentionally minimal)

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


## 4. Testing strategy

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


## 5. API layer (`DevExplorer.Api`)

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


## 6. Frontend (`DevExplorer.Angular` — Angular)

### Scope (keep minimal)
- start indexing job
- show progress + cancel
- search logs
- view log details + context
- basic loading / error states

No complex state management initially.


## 7. Private project (`DevExplorer.Secrets`)

Not committed to GitHub.

Implements:
- `ILogPathPolicy`
- `IDbQueryCatalog`
- `IApiEndpointCatalog`

Contains:
- real folder paths
- real stored procedure names
- internal API endpoints and auth

Public repo ships demo implementations only.


## 8. Development plan (step by step)

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


## 9. Post-project mandatory step

After DevExplorer is complete:

Write short, concise articles:
- C# fundamentals
- Async & TPL
- ASP.NET Core & JWT
- Angular as API client

These articles are part of the learning goal and interview preparation.

## 10. Guiding principles

- clarity over cleverness
- async correctness over micro-optimizations
- real usefulness over architectural purity
- explainable decisions > “best practices”
