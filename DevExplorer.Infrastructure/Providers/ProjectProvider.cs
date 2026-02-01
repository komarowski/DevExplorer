using DevExplorer.Domain.Contracts;
using System.Reflection;

namespace DevExplorer.Infrastructure.Providers;

/// <summary>
/// Static registry for discovering and caching projects.
/// </summary>
public static class ProjectProvider
{
    private static readonly object _lockObj = new();
    private static bool _initialized;
    private static readonly Dictionary<string, IProject> _projects = [];

    /// <summary>Gets a project by name.</summary>
    public static IProject? GetProject(string projectName)
    {
        EnsureInitialized();
        return _projects.TryGetValue(projectName, out var project) ? project : null;
    }

    /// <summary>Gets all registered projects.</summary>
    public static IReadOnlyList<IProject> GetAllProjects()
    {
        EnsureInitialized();
        return _projects.Values.ToList().AsReadOnly();
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
