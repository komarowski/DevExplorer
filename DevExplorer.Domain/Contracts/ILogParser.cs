using DevExplorer.Domain.Models;

namespace DevExplorer.Domain.Contracts;

/// <summary>
/// Parses log data from various sources into structured LogEvents.
/// </summary>
public interface ILogParser
{
    /// <summary>
    /// Parses a single line of text into a LogEvent.
    /// </summary>
    /// <param name="line">A single line of text from a log file</param>
    /// <returns>A parsed LogEvent, or null if the line doesn't complete an entry</returns>
    LogEvent? Parse(string line, string filePath);

    /// <summary>
    /// Reads a log file asynchronously and yields parsed LogEvents.
    /// </summary>
    /// <param name="filePath">Path to the log file to read</param>
    /// <param name="cancellationToken">Token to cancel the read operation</param>
    /// <returns>An async stream of parsed LogEvents</returns>
    IAsyncEnumerable<LogEvent> ReadLogFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads multiple log files and aggregates all parsed events into a ProjectLogEvents.
    /// </summary>
    /// <param name="projectName">The project/application name</param>
    /// <param name="filePaths">Enumerable of file paths to read and parse</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A ProjectLogEvents containing all parsed events from all files</returns>
    Task<ProjectLogEvents> ReadProjectLogsAsync(
        string projectName,
        IEnumerable<string> filePaths,
        CancellationToken cancellationToken = default);
}
