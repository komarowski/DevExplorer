namespace DevExplorer.Domain.Models;

public class ProjectLogEvents
{
    public required string ProjectName { get; set; }

    public List<LogEvent> LogEvents { get; set; } = [];
}
