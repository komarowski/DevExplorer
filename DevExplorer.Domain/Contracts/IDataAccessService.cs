namespace DevExplorer.Domain.Contracts;

/// <summary>
/// Contract for data access operations.
/// </summary>
public interface IDataAccessService
{
    /// <summary>
    /// Executes a SELECT query and returns dynamic results.
    /// </summary>
    Task<List<dynamic>> ExecuteSelectAsync(
        string query,
        object? parameters = null,
        CancellationToken ct = default);

    /// <summary>
    /// Executes a scalar query (returns single value).
    /// </summary>
    Task<T?> ExecuteScalarAsync<T>(
        string query,
        object? parameters = null,
        CancellationToken ct = default);
}
