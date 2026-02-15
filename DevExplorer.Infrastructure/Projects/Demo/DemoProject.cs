using DevExplorer.Domain.Abstractions;
using DevExplorer.Domain.Attributes;
using DevExplorer.Domain.Constants;
using DevExplorer.Domain.Models;
using System.Text.Json;

namespace DevExplorer.Infrastructure.Projects;

/// <summary>
/// Example project demonstrating the plugin pattern.
/// </summary>
[Project(name: "demo")]
public class DemoProject : ProjectBase
{
    public override Task<List<LogEvent>> GetLogsAsync(LogFilter? filter, CancellationToken ct = default)
    {
        var logEventList = new List<LogEvent>
        {
            new() {
                Timestamp = DateTime.Now.AddHours(-1),
                Level = LogLevel.Info,
                Message = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.",
                FilePath = "demo.log",
                Line = 12
            },
            new() {
                Timestamp = DateTime.Now,
                Level = LogLevel.Error,
                Message = "Demo log entry 2",
                FilePath = "demo.log",
                Line = 13
            },
            new() {
                Timestamp = DateTime.Now.AddDays(-1),
                Level = LogLevel.Critical,
                Message = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                FilePath = "demo.log",
                Line = 11
            },
            new() {
                Timestamp = DateTime.Now.AddDays(-5),
                Level = LogLevel.Debug,
                Message = "Demo log entry 4",
                FilePath = "demo.log",
                Line = 19
            }
        };

        if (filter != null)
        {
            var filteredLogEventList = logEventList
                .Where(log => log.Timestamp >= filter.DateFrom && log.Timestamp <= filter.DateTo)
                .OrderByDescending(log => log.Timestamp)
                .ToList();

            return Task.FromResult(filteredLogEventList);
        }

        return Task.FromResult(logEventList);
    }

    [ProjectAction(ProjectActionType.SqlScript, "Retrieves daily summary data")]
    public static async Task<ProjectActionResult> DailySummary(CancellationToken ct = default)
    {
        await Task.Delay(1000, ct);
        var data = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><Summary><Date>{DateTime.Today:yyyy-MM-dd}</Date><Count>42</Count><Status>OK</Status></Summary>";
        return new ProjectActionResult(
            true,
            data,
            MimeTypes.Xml,
            "Daily summary retrieved successfully"
        );
    }

    [ProjectAction(ProjectActionType.ApiEndpoint, "Gets user information")]
    public static async Task<ProjectActionResult> GetUsers(CancellationToken ct = default)
    {
        await Task.Delay(500, ct);
        var data = JsonSerializer.Serialize(new[] { new { Id = 1, Name = "Alice" }, new { Id = 2, Name = "Bob" } });
        return new ProjectActionResult(
            true,
            data,
            MimeTypes.Json,
            "Users retrieved successfully"
        );
    }

    [ProjectAction(ProjectActionType.ApiEndpoint, "Searches products by name")]
    public static async Task<ProjectActionResult> SearchProducts(CancellationToken ct = default)
    {
        await Task.Delay(2000, ct);
        var data = JsonSerializer.Serialize(new[] { new { Id = 1, Name = $"Product: default", Price = 29.99 } });
        return new ProjectActionResult(
            true,
            data,
            MimeTypes.Json,
            null
        );
    }
}
