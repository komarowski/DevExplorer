# DevExplorer — Learning Plan (Interview-Focused)

This document maps **interview topics** to **concrete tasks inside DevExplorer**.

The core principle:
> Implement first → understand by doing → summarize concisely in writing.

DevExplorer is not only a project.
It is also a **knowledge distillation tool**.

---

## 1. OOP in C# (abstract / virtual / sealed)

### What to learn
- Interface vs abstract class
- Purpose of `virtual`
- When to use `sealed`
- Polymorphism and LSP

### How to practice in DevExplorer
- `abstract class LogParserBase`
- `virtual` hooks (`TryParseTimestamp`, `TryExtractCorrelationId`)
- `sealed` concrete parsers
- Unit tests written against interfaces

### Expected outcome
You can explain:
- *why* abstraction is needed
- *where* inheritance helps
- *where* it should be forbidden

---

## 2. Async / Parallel / TPL

### What to learn
- Task vs Thread
- async/await vs parallel execution
- Thread pool behavior
- Bounded concurrency

### How to practice in DevExplorer
- Async file streaming (`IAsyncEnumerable`)
- Producer/consumer pipeline (`Channel`)
- Parallel parsing (`Parallel.ForEachAsync`)
- Compare sync vs async indexing behavior

---

## 3. Thread synchronization

### What to learn
- Race conditions
- Atomic operations
- Concurrent collections
- `lock` vs `Interlocked`

### How to practice in DevExplorer
- Shared index store with concurrent writes
- Progress counters using `Interlocked`
- Intentional broken version → fixed version

---

## 4. Task cancellation

### What to learn
- CancellationToken propagation
- Linked tokens
- Correct handling of `OperationCanceledException`

### How to practice in DevExplorer
- API → Application → Infrastructure token flow
- Cancel indexing job mid-way
- Integration tests validating cancellation behavior

---

## 5. ASP.NET Core fundamentals

### What to learn
- Middleware pipeline order
- Routing and route groups
- Endpoint filters vs middleware
- Minimal API vs controllers

### How to practice in DevExplorer
- CorrelationId middleware
- Global exception handling middleware
- Endpoint filters for validation and timing
- Versioned routes (`/api/v1/...`)

---

## 6. JWT Authentication & Authorization

### What to learn
- JWT structure
- Authentication vs authorization
- Claims, roles, policies

### How to practice in DevExplorer
- Secure endpoints with JWT
- Role-based access:
  - viewer (search)
  - indexer (start/cancel)
- Angular client attaching bearer token

---

## 7. EF Core (high-risk interview topic)

### What to learn
- Tracking vs no-tracking
- Projections
- Raw SQL / stored procedures
- Loading strategies
- Cancellation support

### How to practice in DevExplorer
- Read-only queries with `AsNoTracking`
- DTO projections
- SP access behind `IDbQueryCatalog`
- Integration tests using SQLite

---

## 8. Angular (basic but confident)

### What to learn
- HttpClient
- Component communication
- Async state handling
- Error and loading states

### How to practice in DevExplorer
- Index job UI
- Search UI
- Log detail viewer
- Progressive loading indicators

---

## 9. Study rule (important)

- ❌ Don’t study theory in isolation
- ✅ Implement feature first
- ✅ Then read theory that explains what you already built
- ✅ Then explain it in your own words

If you can’t explain a feature in **5–7 sentences**, you don’t fully understand it yet.

---

## 10. Final mandatory step — Write concise articles (NEW)

After DevExplorer is **functionally complete**, you must write **short, structured articles**.

These articles are:
- **not tutorials**
- **not blog spam**
- **not copied from docs**

They are **compressed explanations** of what *you now actually understand*.

### Why this step is critical
- Interviews test *verbal compression*, not raw coding ability
- Writing forces you to structure mental models
- These texts become:
  - interview prep notes
  - LinkedIn / GitHub material
  - personal reference docs

---

### 10.1 Required articles (recommended set)

Create a folder:

