# C# Fundamentals in Practice

## 1. Interfaces vs abstract classes — what is the real difference?

 - When we talk about **abstract classes** we are defining characteristics of an object type; ***specifying what an object is***.
 - When we talk about an **interface** and define capabilities that we promise to provide, we are talking about establishing a contract about ***what the object can do***.

### Where interfaces help more than abstract classes?

- When implementations are unrelated but need the same contract. Example: `IStorage` implemented by `LocalFileStorage` and `AzureBlobStorage`.
- When you want simple unit tests.
- When you want to avoid shared state and hidden coupling. Example: a base class adds caching/fields and every child silently inherits those constraints.

⭐ Modern C# allows **default interface implementations** (can contain method implementations). Useful for small methods, but keep it minimal so the interface still reads like a contract.

### Where abstract classes help more than interfaces?

- When you have real shared behavior.
- When you want to explicitly manage what can be overridden vs what stays public/stable.

### Why not use class over abstract class?

- A normal class can be created directly. People may start using it as the “default”, even if it’s not meant to be.
- An abstract class can’t be created. It forces you to make a real project class and fill in the missing parts.

📌 In DevExplorer:

- `IProject` (interface) is the plugin boundary: it describes what the rest of the system can rely on.
- `ProjectBase` (abstract class) is a convenience implementation: it provides shared action discovery/execution.
- This gives a clean tradeoff: you can implement `IProject` directly when you need full control, or inherit `ProjectBase` when you want the standard behavior with less code.


## 2. `abstract` vs `virtual` vs `sealed` — why do they exist?

- `abstract` - incomplete on purpose.
	- Where: on **classes** (can’t `new` them) and on **methods/properties** (child must implement).

- `virtual` - has a default, but can be overridden.
	- Where: on **methods/properties**.

- `sealed` - stop inheritance / stop overriding.
	- Where: on **classes**, or on an **override** (`sealed override`) methods/properties.

📌 In DevExplorer:

- `ProjectBase` is `abstract` so every project must be a real concrete type.
- `ProjectBase.HasLogs` is `virtual` so projects can override a safe default.
- `TextParserBase` uses `abstract` hooks for required parsing rules and a `virtual` hook for optional formatting.


## 3. Why do boundaries matter more than inheritance?

In production the hard part is not writing code once — it’s changing code safely.

- **Independent change**: you can replace an implementation without rewriting callers.
- **Testability**: you can test business rules without booting the whole app.
- **Deploy safety**: fewer places to touch means fewer accidental breakages.
- **Clear ownership**: “who is allowed to know about what” becomes obvious.

⭐ Don’t create boundaries for every tiny helper. If the abstraction doesn’t reduce change-risk, it’s just extra code.

📌 In DevExplorer:

- Domain defines the boundary (`IProject`, `IProjectLogParser`, models).
- Infrastructure implements it (projects, parsers).


## 4. `static` — what it is and when to use it

`static` means "belongs to the type, not to a specific object".

- **On a class**: `static class` (can’t be created and can’t be inherited). Good for pure helpers.
- **On fields/properties**: one shared value.
- **On methods**: callable without creating an object.

### Why is it useful?

- **Pure helper functions**: no state, just logic (safe and simple).
- **Shared caches**: when you truly want one cache for the whole process.
- **Global registries**: sometimes acceptable for plugin discovery.

### Where is it NOT useful (or dangerous)?

- **Hidden dependencies**: `static` is reachable from anywhere, so code becomes harder to reason about.
- **Test issues**: shared mutable state can leak between tests.
- **Concurrency issues**: multiple threads hit the same state.

### `static` vs singleton service (DI) — what’s the difference?

Singleton service (DI): one instance, but you can replace it for tests, configure it, and control its lifetime via the container.

- Prefer **singleton service** for "application behavior".
- Prefer **static** for "pure helpers" (and only very careful, thread-safe caches).

### `static readonly` vs `const` — what’s the difference?

- `const` - Compile-time constant.
	- Best for true constants that never change (like `Math.PI`, small fixed strings).

- `static readonly` - Runtime constant: assigned once (often at startup).
	- Best for values that might change later, or values that aren’t compile-time constants (like `DateTime`, complex objects, computed strings).

📌 DevExplorer example:

- `ProjectProvider` is `static` so the app can discover projects once and reuse the results everywhere.


## 5. `struct` vs `record` vs `class` — when to use which

- `struct` is a **value type** (copied by value). Best for small, simple, usually-immutable values.
- `record` is usually for **data/messages** with value-based equality (great for results/metadata).
- `class` is usually for **mutable objects** or things with identity/lifecycle.

### When should I choose `struct`?

- When the type is small (a few fields), used a lot, and you want value semantics.
- When you can keep it immutable.

### When should I choose `record`?

- When the type is a **message / DTO / result / metadata**.
- When you want easy equality (good for tests and comparisons).

📌 DevExplorer examples:

- `ProjectAction` is a `record`: metadata (name/type/description).
- `ProjectActionResult` is a `record`: a result value (success + payload).

### When should I choose `class`?

- When the object is **mutable** (you fill it step-by-step).
- When it has identity/lifecycle (you pass the same instance around).

📌 DevExplorer examples:

- `LogEvent` is a `class`: it gets filled/updated during parsing (`FilePath`, `Line`).
- `LogFilter` is a `class`: it’s a mutable input object (`DateFrom`, `DateTo`).

### Where can it go wrong?

- `struct` that is large or mutable can hurt performance (copies) and becomes confusing.
- `record` that is mutated a lot is confusing ("equal" can change as values change).
- `class` for pure data can make equality/tests noisy.
