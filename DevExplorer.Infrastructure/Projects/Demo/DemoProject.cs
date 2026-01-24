using DevExplorer.Domain.Contracts;
using DevExplorer.Domain.Models;

namespace DevExplorer.Infrastructure.Projects;

/// <summary>
/// Example public project for the "standard" project.
/// Demonstrates the Project plugin pattern with different methods (text logs, database, API).
/// </summary>
public class DemoProject : IProject
{
    public string Name => "demo";

    public Task<List<LogEvent>> GetLogsAsync(LogFilter filter, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<LogEvent>> QueryAsync(string query, CancellationToken ct = default)
    {
        // In a real implementation, use IEnrichmentProvider to look up database data
        await Task.Delay(100, ct);
        return new List<LogEvent>();
    }

    public async Task<string> CallApiAsync(string endpoint, CancellationToken ct = default)
    {
        // In a real implementation, use IApiEndpointCatalog to map logical endpoints to real ones
        await Task.Delay(100, ct);
        return "{}";
    }
}
