using DevExplorer.Domain.Models;
using DevExplorer.Infrastructure.Providers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// List all available projects
app.MapGet("/projects", () =>
{
    var projects = ProjectProvider.GetAllProjects();
    var projectList = projects
        .Select(p => new
        {
            p.Name,
            p.HasLogs,
            actions = p.GetProjectActions()
        })
        .ToList();

    return Results.Ok(projectList);
});

// TODO: Why we need CancellationToken here?

// GET /projects/{projectName}/actions/{actionName} - Execute a project action
app.MapGet("/projects/{projectName}/actions/{actionName}", async (string projectName, string actionName, CancellationToken ct) =>
{
    var project = ProjectProvider.GetProject(projectName.ToLower());
    if (project == null)
        return Results.NotFound($"Project '{projectName}' not found");

    var actionResult = await project.ExecuteProjectActionAsync(actionName.ToLower(), ct);

    return Results.Ok(actionResult);
});

// POST /projects/{projectName}/logs - Retrieve logs with optional date filter
app.MapPost("/projects/{projectName}/logs", async (string projectName, [FromBody] LogFilter? filter, CancellationToken ct) =>
{
    var project = ProjectProvider.GetProject(projectName.ToLower());
    if (project == null)
        return Results.NotFound($"Project '{projectName}' not found");

    var logs = await project.GetLogsAsync(filter, ct);

    return Results.Ok(logs);
});

// Serve static files and fallback to index.html for React SPA routing
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.Run();
