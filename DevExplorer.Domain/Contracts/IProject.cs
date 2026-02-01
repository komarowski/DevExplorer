using DevExplorer.Domain.Models;

namespace DevExplorer.Domain.Contracts;

/// <summary>
/// Plugin contract for projects.
/// </summary>
public interface IProject
{
    string Name { get; }

    bool HasLogs { get; }

    Task<List<LogEvent>> GetLogsAsync(LogFilter? filter, CancellationToken ct = default);

    IReadOnlyList<ProjectAction> GetProjectActions();

    Task<ProjectActionResult> ExecuteProjectActionAsync(string actionName, CancellationToken ct = default);
}