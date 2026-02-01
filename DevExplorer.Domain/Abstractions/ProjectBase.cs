using System.Reflection;
using DevExplorer.Domain.Attributes;
using DevExplorer.Domain.Contracts;
using DevExplorer.Domain.Models;

namespace DevExplorer.Domain.Abstractions;

/// <summary>
/// Base class for project implementations.
/// </summary>
public abstract class ProjectBase : IProject
{
    private IReadOnlyList<ProjectAction>? _cachedActions;
    private MethodInfo[]? _cachedMethods;

    /// <summary>Unique identifier for the project.</summary>
    public abstract string Name { get; }

    /// <summary>Indicates whether the project provides logs.</summary>
    public virtual bool HasLogs => false;

    /// <summary>Get logs from the project.</summary>
    public abstract Task<List<LogEvent>> GetLogsAsync(LogFilter? filter, CancellationToken ct = default);

    /// <summary>
    /// Discovers methods marked with [ProjectAction] attribute and returns them as ProjectAction records.
    /// Results are cached after first call.
    /// </summary>
    public IReadOnlyList<ProjectAction> GetProjectActions()
    {
        if (_cachedActions != null)
            return _cachedActions;

        var actions = new List<ProjectAction>();
        var projectMethods = GetCachedMethods();

        foreach (var method in projectMethods)
        {
            var attribute = method.GetCustomAttribute<ProjectActionAttribute>();
            if (attribute != null)
            {
                actions.Add(new ProjectAction(
                    method.Name.ToLower(),
                    attribute.ActionType,
                    attribute.Description));
            }
        }

        _cachedActions = actions.AsReadOnly();
        return _cachedActions;
    }

    /// <summary>
    /// Executes a project action by name with optional cancellation token.
    /// Methods are cached for performance.
    /// </summary>
    public Task<ProjectActionResult> ExecuteProjectActionAsync(string actionName, CancellationToken ct = default)
    {
        var method = GetCachedMethods()
            .FirstOrDefault(m => m.Name.Equals(actionName, StringComparison.CurrentCultureIgnoreCase));

        if (method == null)
        {
            return Task.FromResult(new ProjectActionResult(false, null, null, "Action not found"));
        }

        try
        {
            return (Task<ProjectActionResult>)method.Invoke(this, [ct])!;
        }
        catch (Exception ex)
        {
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            return Task.FromResult(new ProjectActionResult(false, null, null, $"Error executing action: {errorMessage}"));
        }
    }

    /// <summary>
    /// Gets cached public instance methods for this type.
    /// </summary>
    private MethodInfo[] GetCachedMethods()
    {
        if (_cachedMethods != null)
            return _cachedMethods;

        _cachedMethods = GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);

        return _cachedMethods;
    }
}
