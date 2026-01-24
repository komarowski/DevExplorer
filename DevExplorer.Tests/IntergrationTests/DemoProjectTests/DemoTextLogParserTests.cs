using DevExplorer.Domain.Contracts;
using DevExplorer.Domain.Models;
using DevExplorer.Infrastructure.Projects;

namespace DevExplorer.Tests.IntergrationTests.DemoProjectTests;

/// <summary>
/// Integration tests for TextLogParser: creates log files, reads them, and verifies parsing.
/// </summary>
public class DemoTextLogParserTests : IDisposable
{
    private readonly string _testLogDirectory;
    private readonly IProjectLogParser _parser;

    public DemoTextLogParserTests()
    {
        _testLogDirectory = Path.Combine(Path.GetTempPath(), $"DevExplorer_Tests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testLogDirectory);
        _parser = new DemoTextLogParser();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testLogDirectory))
            Directory.Delete(_testLogDirectory, recursive: true);
    }

    [Fact]
    public async Task ReadAndParseSingleLogFile_ShouldParseMultipleLogEvents()
    {
        // Arrange
        var logContent = """
            2026-01-11 10:30:45 INFO Application started
            2026-01-11 10:30:46 DEBUG Initializing components
            2026-01-11 10:30:47 ERROR Failed to connect to database
            Connection timeout after 30 seconds
            2026-01-11 10:30:48 WARNING Retrying connection
            """;

        var logFilePath = Path.Combine(_testLogDirectory, "test.log");
        await File.WriteAllTextAsync(logFilePath, logContent);

        // Act
        var logEvents = new List<LogEvent>();
        await foreach (var logEvent in _parser.ReadLogFileAsync(logFilePath))
        {
            logEvents.Add(logEvent);
        }

        // Assert
        Assert.Equal(4, logEvents.Count);

        // First event: INFO
        Assert.Equal(LogLevel.Info, logEvents[0].Level);
        Assert.Equal("Application started", logEvents[0].Message);
        Assert.NotEqual(default, logEvents[0].Timestamp);

        // Second event: DEBUG
        Assert.Equal(LogLevel.Debug, logEvents[1].Level);
        Assert.Equal("Initializing components", logEvents[1].Message);

        // Third event: ERROR with continuation
        Assert.Equal(LogLevel.Error, logEvents[2].Level);
        Assert.Contains("Failed to connect to database", logEvents[2].Message);
        Assert.Contains("Connection timeout", logEvents[2].Message);

        // Fourth event: WARNING
        Assert.Equal(LogLevel.Warning, logEvents[3].Level);
        Assert.Equal("Retrying connection", logEvents[3].Message);
    }

    [Fact]
    public async Task ReadAndParseMultipleFiles_ShouldReturnProjectLogEvents()
    {
        // Arrange
        var appLogContent = """
            2026-01-11 10:30:45 INFO App started
            2026-01-11 10:30:46 ERROR App error
            """;

        var errorLogContent = """
            2026-01-11 10:30:47 CRITICAL System failure
            2026-01-11 10:30:48 INFO Recovery initiated
            """;

        var appLogPath = Path.Combine(_testLogDirectory, "app.log");
        var errorLogPath = Path.Combine(_testLogDirectory, "error.log");

        await File.WriteAllTextAsync(appLogPath, appLogContent);
        await File.WriteAllTextAsync(errorLogPath, errorLogContent);

        // Act
        var projectEvents = await _parser.ReadProjectLogsAsync(
            "TestProject",
            [appLogPath, errorLogPath]
        );

        // Assert
        Assert.Equal("TestProject", projectEvents.ProjectName);
        Assert.Equal(4, projectEvents.LogEvents.Count);

        // Verify events from both files are included
        var infoEvents = projectEvents.LogEvents.Where(e => e.Level == LogLevel.Info).ToList();
        var errorEvents = projectEvents.LogEvents.Where(e => e.Level == LogLevel.Error).ToList();
        var criticalEvents = projectEvents.LogEvents.Where(e => e.Level == LogLevel.Critical).ToList();

        Assert.Equal(2, infoEvents.Count);
        Assert.Single(errorEvents);
        Assert.Single(criticalEvents);
    }

    [Fact]
    public async Task ParseDifferentLogLevels_ShouldMapCorrectly()
    {
        // Arrange
        var logContent = """
            2026-01-11 10:30:45 TRACE Trace message
            2026-01-11 10:30:46 DEBUG Debug message
            2026-01-11 10:30:47 INFO Info message
            2026-01-11 10:30:48 WARN Warn message
            2026-01-11 10:30:49 WARNING Warning message
            2026-01-11 10:30:50 ERROR Error message
            2026-01-11 10:30:51 CRITICAL Critical message
            2026-01-11 10:30:52 FATAL Fatal message
            """;

        var logFilePath = Path.Combine(_testLogDirectory, "levels.log");
        await File.WriteAllTextAsync(logFilePath, logContent);

        // Act
        var logEvents = new List<LogEvent>();
        await foreach (var logEvent in _parser.ReadLogFileAsync(logFilePath))
        {
            logEvents.Add(logEvent);
        }

        // Assert
        Assert.Equal(LogLevel.Trace, logEvents[0].Level);
        Assert.Equal(LogLevel.Debug, logEvents[1].Level);
        Assert.Equal(LogLevel.Info, logEvents[2].Level);
        Assert.Equal(LogLevel.Warning, logEvents[3].Level);
        Assert.Equal(LogLevel.Warning, logEvents[4].Level); // WARN maps to Warning
        Assert.Equal(LogLevel.Error, logEvents[5].Level);
        Assert.Equal(LogLevel.Critical, logEvents[6].Level); // CRITICAL
        Assert.Equal(LogLevel.Critical, logEvents[7].Level); // FATAL maps to Critical
    }

    [Fact]
    public async Task ParseMultiLineStackTrace_ShouldCombineIntoSingleEvent()
    {
        // Arrange
        var logContent = """
            2026-01-11 10:30:47 ERROR Exception occurred
            System.NullReferenceException: Object reference not set to an instance of an object.
               at DevExplorer.Core.DoSomething() in FileLogSource.cs:line 42
               at DevExplorer.Api.ProcessRequest() in Program.cs:line 15
            2026-01-11 10:30:48 INFO Continuing execution
            """;

        var logFilePath = Path.Combine(_testLogDirectory, "stacktrace.log");
        await File.WriteAllTextAsync(logFilePath, logContent);

        // Act
        var logEvents = new List<LogEvent>();
        await foreach (var logEvent in _parser.ReadLogFileAsync(logFilePath))
        {
            logEvents.Add(logEvent);
        }

        // Assert
        Assert.Equal(2, logEvents.Count);

        var errorEvent = logEvents[0];
        Assert.Equal(LogLevel.Error, errorEvent.Level);
        Assert.Contains("Exception occurred", errorEvent.Message);
        Assert.Contains("System.NullReferenceException", errorEvent.Message);
        Assert.Contains("at DevExplorer.Core.DoSomething", errorEvent.Message);
        Assert.Contains("at DevExplorer.Api.ProcessRequest", errorEvent.Message);

        var infoEvent = logEvents[1];
        Assert.Equal(LogLevel.Info, infoEvent.Level);
        Assert.Equal("Continuing execution", infoEvent.Message);
    }
}