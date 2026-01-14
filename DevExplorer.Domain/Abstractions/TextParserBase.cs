using DevExplorer.Domain.Contracts;
using DevExplorer.Domain.Models;
using System.Runtime.CompilerServices;

namespace DevExplorer.Domain.Abstractions;

/// <summary>
/// Abstract base parser for text-based log files with multi-line entry support.
/// 
/// DESIGN NOTES:
/// - Uses buffering to handle multi-line entries (e.g., stack traces, wrapped text)
/// - Streaming approach: yields events as parsed, not loading entire file into memory
/// - Supports both single-file and multi-file reading with async/await
/// - Uses IAsyncEnumerable for memory efficiency and LINQ composability
/// - CancellationToken support allows clients to cancel long-running operations
/// 
/// INHERITANCE:
/// Subclasses must override IsNewEntryLine() and ParseNewEntry() to customize
/// parsing for different log formats (text, JSON, CSV, etc.).
/// </summary>
public abstract class TextParserBase : ILogParser
{
    private LogEvent? _bufferedEvent;

    /// <summary>
    /// Parses a single line of text and returns a LogEvent if a complete entry is detected.
    /// </summary>
    public LogEvent? Parse(string line, string filePath)
    {
        if (IsNewEntryLine(line))
        {
            var previousEvent = _bufferedEvent;
            _bufferedEvent = ParseNewEntry(line, filePath);
            return previousEvent;
        }

        if (_bufferedEvent != null)
        {
            AppendToMessage(_bufferedEvent, line);
            return null;
        }

        return null;
    }

    /// <summary>
    /// Retrieves and clears the currently buffered event.
    /// </summary>
    public LogEvent? Flush()
    {
        var result = _bufferedEvent;
        _bufferedEvent = null;
        return result;
    }

    /// <summary>
    /// Reads a log file asynchronously and yields parsed LogEvents as they're read.
    /// </summary>
    public async IAsyncEnumerable<LogEvent> ReadLogFileAsync(
        string filePath,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Log file not found: {filePath}");

        using var reader = new StreamReader(filePath);
        string? line;
        int lineNumber = 1;

        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            var logEvent = Parse(line, filePath);
            if (logEvent != null)
            {
                logEvent.FilePath = filePath;
                logEvent.Line = lineNumber;
                yield return logEvent;
            }

            lineNumber++;
        }

        var lastEvent = Flush();
        if (lastEvent != null)
        {
            lastEvent.FilePath = filePath;
            lastEvent.Line = lineNumber;
            yield return lastEvent;
        }
    }

    /// <summary>
    /// Reads multiple log files and aggregates all parsed events into a ProjectLogEvents.
    /// </summary>
    public async Task<ProjectLogEvents> ReadProjectLogsAsync(
        string projectName,
        IEnumerable<string> filePaths,
        CancellationToken cancellationToken = default)
    {
        var projectEvents = new ProjectLogEvents { ProjectName = projectName };

        foreach (var filePath in filePaths)
        {
            await foreach (var logEvent in ReadLogFileAsync(filePath, cancellationToken))
            {
                projectEvents.LogEvents.Add(logEvent);
            }
        }

        return projectEvents;
    }

    /// <summary>
    /// Determines if a line starts a new log entry.
    /// </summary>
    protected abstract bool IsNewEntryLine(string line);

    /// <summary>
    /// Parses a line that begins a new log entry and creates a LogEvent.
    /// </summary>
    protected abstract LogEvent ParseNewEntry(string line, string filePath);

    /// <summary>
    /// Appends a continuation line to an existing LogEvent's message.
    /// </summary>
    protected virtual void AppendToMessage(LogEvent logEvent, string continuationLine)
    {
        logEvent.Message += "\n" + continuationLine;
    }
}
