using DevExplorer.Domain.Models;

namespace DevExplorer.Domain.Contracts;

/// <summary>
/// Plugin contract for projects.
/// </summary>
public interface IProject
{
    string Name { get; }

    string DisplayName { get; }

    bool HasLogs { get; }

    bool HasDatabase { get; }

    bool HasDomainUrl { get; }

    Task<List<LogEvent>> GetLogsAsync(LogFilter? filter, CancellationToken ct = default);

    IReadOnlyList<ProjectAction> GetProjectActions();

    Task<ProjectActionResult> ExecuteProjectActionAsync(string actionName, CancellationToken ct = default);
}