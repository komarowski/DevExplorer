using DevExplorer.Domain.Constants;

namespace DevExplorer.Domain.Models;

/// <summary>
/// Represents a single log entry.
/// </summary>
public class LogEvent
{
    public DateTime Timestamp { get; set; }

    public LogLevel Level { get; set; }

    public string Message { get; set; } = string.Empty;

    public required string FilePath { get; set; }

    public int Line { get; set; }
}
