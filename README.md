# DevExplorer

DevExplorer is a **local developer tool** for exploring and correlating technical data from multiple sources:

- file-based logs
- database queries
- external APIs

The project is built as an **async-first .NET application** with a clean, pragmatic architecture.
Its goals are twofold:

1. Be genuinely useful for debugging and investigation tasks
2. Serve as a practical learning project for modern .NET backend development

Key principles:
- async and cancellation-aware by default
- bounded concurrency and safe parallelism
- clear separation between core logic, API, and UI
- no overengineering — patterns are added only when justified

Sensitive or environment-specific logic (paths, stored procedure names, internal endpoints)
is isolated behind abstractions and can live in a private library.

The repository also acts as an interview preparation project,
covering C# fundamentals, async/TPL, ASP.NET Core, JWT, EF Core, and frontend integration.
