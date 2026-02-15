using DevExplorer.Domain.Contracts;
using System.Reflection;

namespace DevExplorer.Infrastructure.Providers;

/// <summary>
/// Static registry for discovering project implementations and matching them with JSON config.
/// JSON settings define which projects are available per environment.
/// </summary>
public static class ProjectProvider
{
    private static readonly object _lockObj = new();
    private static bool _initialized;
    private static readonly Dictionary<string, IProject> _projects = new(StringComparer.OrdinalIgnoreCase);

    private static IProjectSettingsService? _settingsService;
    

    /// <summary>
    /// Sets the project settings service. Must be called during app startup.
    /// </summary>
    public static void SetProjectSettingsService(IProjectSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <summary>
    /// Gets a project by name(сase-insensitive) if it's configured for current environment and has implementation.
    /// </summary>
    public static IProject? GetProject(string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName))
            return null;

        EnsureInitialized();

        var settings = _settingsService?.GetProjectSettings(projectName);
        if (settings == null)
            return null; // Not configured for current environment

        return _projects.GetValueOrDefault(projectName);
    }

    /// <summary>
    /// Gets all projects that are configured for current environment AND have implementations.
    /// </summary>
    public static IReadOnlyList<IProject> GetAllProjects()
    {
        EnsureInitialized();

        if (_settingsService == null)
            return [];

        var configuredProjects = _settingsService.GetAllProjectSettings();
        var result = new List<IProject>();

        foreach (var config in configuredProjects)
        {
            if (_projects.TryGetValue(config.Name, out var implementation))
            {
                result.Add(implementation);
            }
        }

        return result.AsReadOnly();
    }

    /// <summary>
    /// Gets project names that are configured but don't have implementations.
    /// Useful for diagnostics.
    /// </summary>
    public static IReadOnlyList<string> GetMissingImplementations()
    {
        EnsureInitialized();

        if (_settingsService == null)
            return [];

        return _settingsService.GetAllProjectSettings()
            .Where(config => !_projects.ContainsKey(config.Name))
            .Select(config => config.Name)
            .ToList()
            .AsReadOnly();
    }

    private static void EnsureInitialized()
    {
        if (_initialized)
            return;

        lock (_lockObj)
        {
            if (_initialized)
                return;

            DiscoverProjects();
            _initialized = true;
        }
    }

    private static void DiscoverProjects()
    {
        var projectTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IProject).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var projectType in projectTypes)
        {
            try
            {
                if (Activator.CreateInstance(projectType) is IProject project)
                {
                    _projects[project.Name] = project;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create project {projectType.Name}: {ex.Message}");
            }
        }
    }
}
