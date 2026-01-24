using DevExplorer.Domain.Models;
using DevExplorer.Infrastructure.Providers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/projects", () =>
{
    var projects = ProjectProvider.GetAllProjects();
    var projectList = projects
        .Select(p => new { p.Name })
        .ToList();

    return Results.Ok(projectList);
});

app.MapPost("/projects/{projectName}/logs", async (string projectName, [FromBody] LogFilter? filter, CancellationToken ct) =>
{
    var project = ProjectProvider.GetProject(projectName.ToLower());
    if (project == null)
        return Results.NotFound($"Project '{projectName}' not found");

    var logs = await project.GetLogsAsync(filter, ct);
    return Results.Ok(logs);
});

app.UseStaticFiles();

// Fallback to index.html for SPA routing
app.MapFallbackToFile("index.html");

app.Run();
