using DevExplorer.Domain.Models;

namespace DevExplorer.Domain.Contracts;

/// <summary>
/// Minimal interface for a project plugin.
/// </summary>
public interface IProject
{
    /// <summary>Unique identifier for the project.</summary>
    string Name { get; }

    Task<List<LogEvent>> GetLogsAsync(LogFilter? filter, CancellationToken ct = default);

    // TODO: GetAvailableMethodsAsync() to return list of ready scenarios of SPs or API endpoints?

    // TODO: CallMethodAsync(string methodName, ??? )
}
