using DevExplorer.Domain.Abstractions;
using DevExplorer.Domain.Contracts;
using DevExplorer.Domain.Models;
using DevExplorer.Infrastructure.Data;
using DevExplorer.Infrastructure.Providers;
using DevExplorer.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configure project settings from appsettings.json
builder.Services.Configure<EnvironmentSettingsOptions>(builder.Configuration.GetSection(EnvironmentSettingsOptions.SectionName));
builder.Services.AddSingleton<IProjectSettingsService, ProjectSettingsService>();

var app = builder.Build();

// Initialize project services
var settingsService = app.Services.GetRequiredService<IProjectSettingsService>();
ProjectBase.SetProjectSettingsService(settingsService);
ProjectBase.SetDataAccessServiceFactory(new DataAccessServiceFactory());
ProjectProvider.SetProjectSettingsService(settingsService);

// GET - Get current environment and available environments
app.MapGet("/environment", (IProjectSettingsService settings) =>
{
    return Results.Ok(new
    {
        current = settings.CurrentEnvironment,
        available = settings.AvailableEnvironments
    });
});

// PUT - Change current environment
app.MapPut("/environment", (IProjectSettingsService settings, [FromBody] string environment) =>
{
    if (!settings.AvailableEnvironments.Contains(environment, StringComparer.OrdinalIgnoreCase))
        return Results.BadRequest($"Environment '{environment}' is not configured");

    settings.CurrentEnvironment = environment;
    return Results.Ok(new { current = settings.CurrentEnvironment });
});

// GET - List all available projects
app.MapGet("/projects", () =>
{
    var projects = ProjectProvider.GetAllProjects();
    var projectList = projects
        .Select(p => new
        {
            p.Name,
            p.DisplayName,
            p.HasLogs,
            p.HasDatabase,
            p.HasDomainUrl,
            actions = p.GetProjectActions()
        })
        .ToList();

    return Results.Ok(projectList);
});

// GET - Execute a project action
app.MapGet("/projects/{projectName}/actions/{actionName}", async (string projectName, string actionName, CancellationToken ct) =>
{
    var project = ProjectProvider.GetProject(projectName.ToLower());
    if (project == null)
        return Results.NotFound($"Project '{projectName}' not found");

    var actionResult = await project.ExecuteProjectActionAsync(actionName.ToLower(), ct);

    return Results.Ok(actionResult);
});

// POST - Retrieve logs with optional date filter
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
