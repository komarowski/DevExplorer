using System.Text.RegularExpressions;
using DevExplorer.Domain.Abstractions;
using DevExplorer.Domain.Models;

namespace DevExplorer.Infrastructure.Projects;

/// <summary>
/// Test implementation of TextLogParser for integration tests.
/// Parses standard text log format: "YYYY-MM-DD HH:mm:ss LEVEL message"
/// Handles multi-line entries (stack traces, wrapped text) via buffering in TextParserBase.
/// </summary>
public partial class DemoTextLogParser : TextParserBase
{
    private static readonly Regex LogEntryPattern = LogEntryPatternRegex();

    protected override bool IsNewEntryLine(string line)
    {
        return !string.IsNullOrWhiteSpace(line) && LogEntryPattern.IsMatch(line);
    }

    protected override LogEvent ParseNewEntry(string line, string filePath)
    {
        var text = line;
        var match = LogEntryPattern.Match(text);

        var dateStr = match.Groups[1].Value;
        var timeStr = match.Groups[2].Value;
        var levelStr = match.Groups[3].Value.ToUpper();

        var timestamp = ParseTimestamp(dateStr, timeStr);
        var level = ParseLevel(levelStr);
        var message = text[match.Length..].Trim();

        return new LogEvent
        {
            Timestamp = timestamp,
            Level = level,
            FilePath = filePath,
            Message = message
        };
    }

    private static DateTime ParseTimestamp(string dateStr, string timeStr)
    {
        if (DateTime.TryParse($"{dateStr} {timeStr}", out var result))
            return result;
        return DateTime.Now;
    }

    private static LogLevel ParseLevel(string levelStr) => levelStr switch
    {
        "TRACE" => LogLevel.Trace,
        "DEBUG" => LogLevel.Debug,
        "INFO" => LogLevel.Info,
        "WARN" or "WARNING" => LogLevel.Warning,
        "ERROR" => LogLevel.Error,
        "CRITICAL" or "FATAL" => LogLevel.Critical,
        _ => LogLevel.Info
    };

    [GeneratedRegex(@"^(\d{4}[-/]\d{1,2}[-/]\d{1,2})\s+(\d{1,2}:\d{2}:\d{2})\s+(TRACE|DEBUG|INFO|WARNING|WARN|ERROR|CRITICAL|FATAL)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex LogEntryPatternRegex();
}
