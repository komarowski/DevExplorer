# Async and Parallelism in .NET


## 1. Async concurrency vs parallel execution — what’s the real difference?

 - **Async** makes better use of resources by making sure a thread gets less/no downtime while *waiting*.
 - **Parallel programming** makes better use of resources by running multiple pieces of *work* at the same time.
 - You can do one or the other — or both together.


## 2. Where async concurrency helps (and why)?

- **ASP.NET Core**: **scalability** — async controllers (`Task`/`Task<T>`) return the request thread to the **thread pool** while DB/API calls are waiting. Without that, enough slow calls can starve the pool.
- **Console/worker apps**: **throughput + cancellation** — run many I/O operations concurrently without creating lots of threads, and support clean shutdown (`Ctrl+C` → `CancellationToken`).
- **Desktop (WPF/WinForms)**: **responsiveness** — avoid blocking the UI thread so the app doesn’t freeze during I/O.

📌 Practical ASP.NET Core guidance:

- Use async all the way for I/O: controller → service → data access.
- Avoid mixing sync + async (especially `.Result` / `.Wait()`).
- Pass a `CancellationToken` from the request to downstream calls.


### Workflow: React → GET /summary → 2 DB calls → response

```csharp
[HttpGet("summary/{userId:int}")]
public async Task<SummaryDto> GetSummary(int userId, CancellationToken ct)
{
	// Start both I/O operations (they are now “in flight”).
	Task<User> userTask = _users.GetUserAsync(userId, ct);              // ~5 seconds
	Task<List<Order>> ordersTask = _orders.GetOrdersAsync(userId, ct);  // ~5 seconds

	// Await both:
	// - They run concurrently (total time ~5s instead of ~10s if awaited one-by-one).
	// - While waiting, ASP.NET Core can return the request thread to the thread pool.
	await Task.WhenAll(userTask, ordersTask);

	// After await, results are available (no extra waiting here).
	User user = userTask.Result;
	List<Order> orders = ordersTask.Result;

	// CPU work runs on a thread pool thread
	var score = CalculateScore(user, orders);
	return new SummaryDto(user.Name, orders.Count, score);
}
```

📌 `Task.WhenAll` pros / cons:

- ✅ **Faster end-to-end latency** when the operations are independent (waits overlap).
- ✅ **Same scalability benefit** as any `await` (doesn’t block threads while waiting).
- ✅ **Fine for a small, fixed set** (e.g., 2–10 independent calls); otherwise use **bounded parallelism**.
- ⚠️ **Failure behavior**: if **any** task fails, `Task.WhenAll` also fails (and if multiple tasks fail, you may see aggregated exceptions rather than “first failure only”).


### Why unbounded parallelism is dangerous?

```csharp
// ⚠️ Looks fine… until items is 10k and each task hits the database.
await Task.WhenAll(items.Select(item => DoWorkAsync(item, ct)));
```

- **Thread pool saturation**: too many runnable tasks compete for threads.
- **Connection pool exhaustion**: DB pools are finite; requests start timing out.
- **File handle / socket exhaustion**: OS resources are not infinite.
- **Memory pressure**: every task allocates state; queues grow.


### What is Thread pool?

The **thread pool** is a shared set of worker threads that .NET uses to run short-lived work: **Thread pool threads are finite**. If you block them, requests queue up.


### What is CancellationToken?

A `CancellationToken` is how you stop *waiting* work (DB/HTTP/file reads) when the user cancels or a request aborts.

Ater cancellation:

- `ct.IsCancellationRequested` becomes `true`.
- If the awaited API supports cancellation, the `Task` typically completes as **Canceled** and `await` throws `OperationCanceledException` (often `TaskCanceledException`).
- `finally` blocks still run (cleanup still happens).
- If an API ignores the token (or you didn’t pass it), the work may keep running.


### What is Task

A `Task` is a **placeholder** for an operation that finishes later.

- **I/O tasks**: the OS waits for you, so your thread is free until the result is ready.
- **CPU tasks**: `Task.Run` schedules it on a **thread pool thread** (mainly useful to keep a **UI thread** responsive, and usually not useful in ASP.NET Core controllers).


## 3. What is Parallel?

Async helps when you’re **waiting** (DB/HTTP/file). Parallelism helps when you’re **working** (CPU).

⭐ Example: You’re indexing 100 log files. Each file needs:

 - **I/O**: read from disk (mostly waiting)
 - **CPU**: parse lines into events (real work)
 - **I/O**: write results to DB/cache (mostly waiting)

If you use only async, you can overlap the *waiting* parts, but the **CPU parse still runs one file at a time** on one core and becomes the bottleneck.

Best .NET syntax to organize it (bounded, simple):

```csharp
await Parallel.ForEachAsync(filePaths,
	new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = ct },
	async (filePath, token) =>
	{
		// I/O: async wait (doesn't block threads)
		string[] lines = await File.ReadAllLinesAsync(filePath, token);

		// CPU: real work (runs on thread pool threads, bounded by MaxDegreeOfParallelism)
		var events = ParseLogLines(lines);

		// I/O: async wait
		await _store.WriteAsync(events, token);
	});
```


## 4. Mutex and Semaphores


### Mutex (mutual exclusion)

**Mutex** is a lock that allows **exactly one owner at a time**. It prevents **two things from modifying the same shared resource** at the same time.

⭐ Example: you have a Windows desktop app + a scheduled task, and both might run and write to the **same local cache folder / SQLite file**. A named `Mutex` ensures only one process performs the critical update.


### What is `lock` in C#?

**lock** is a synchronization mechanism used to ensure that **only one thread** can execute a specific block of code at any given time.

- Use **`lock`** to protect **shared in-memory data** (like a `Dictionary`, `List`, counters).
- Use a **named `Mutex`** to protect a resource **shared by multiple processes** (like a file/SQLite DB).

⭐ Example (`lock` protects a shared dictionary):

```csharp
private static readonly object _cacheLock = new();
private static readonly Dictionary<int, string> _names = new();

public static void AddOrUpdateName(int id, string name)
{
	lock (_cacheLock) // only one thread can edit _names at a time
	{
		_names[id] = name;
	}
}
```

Notes (why not always use `lock` for a dictionary):

- **Performance**: it serializes access (even readers wait), which can become a bottleneck under load.
- **Correctness risk**: *every* read/write must use the same lock; one “forgotten” access reintroduces race bugs.
- **Async limitation**: you can’t `await` inside a `lock` block.


### Semaphore / SemaphoreSlim

**Semaphore** is a "gate" used to **control the number of threads that can access a shared resource or a section of code concurrently**.

⭐ Example: you have a list of 1,000 URLs to fetch (or 1,000 customer IDs to sync). The server allows only a few concurrent requests. Without a semaphore you can spike to hundreds of in-flight calls → `429 Too Many Requests` + timeouts.

Use `SemaphoreSlim` for **async code inside one process**:

```csharp
// Allow at most 8 concurrent requests (everything else waits).
var throttler = new SemaphoreSlim(initialCount: 8, maxCount: 8);

// Note: this is NOT strict "batches of 8".
// It's a sliding window: as soon as 1 finishes and releases, another starts.
await Task.WhenAll(urls.Select(async url =>
{
	// Wait until a slot is free (doesn't block a thread while waiting).
	await throttler.WaitAsync(ct);
	try
	{
		// The bounded operation (example: HTTP request).
		string html = await _http.GetStringAsync(url, ct);
		await SaveToDiskAsync(url, html, ct);
	}
	finally
	{
		// Always release, even if the API call fails/cancels.
		throttler.Release();
	}
}));
```
