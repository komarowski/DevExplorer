namespace DevExplorer.Domain.Models;

public class LogEvent
{
    public DateTime Timestamp { get; set; }

    public LogLevel Level { get; set; }

    public string Message { get; set; } = string.Empty;

    public required string FilePath { get; set; }

    public int Line { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public enum LogLevel
{
    Trace,
    Debug,
    Info,
    Warning,
    Error,
    Critical
}
