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
    private string? _cachedName;
    private static IDataAccessServiceFactory? _factory;
    private static IProjectSettingsService? _settingsService;

    /// <summary>Unique identifier for the project.</summary>
    public string Name
    {
        get
        {
            if (_cachedName != null)
                return _cachedName;

            var attribute = GetType().GetCustomAttribute<ProjectAttribute>();
            if (attribute == null)
                throw new InvalidOperationException($"Project class '{GetType().Name}' must be decorated with [Project(name)] attribute.");

            _cachedName = attribute.Name;
            return _cachedName;
        }
    }

    /// <summary>Display name for UI (falls back to Name).</summary>
    public string DisplayName => Settings?.DisplayName ?? Name;

    /// <summary>True if LogDirectory is configured for current environment.</summary>
    public bool HasLogs => !string.IsNullOrEmpty(Settings?.LogDirectory);

    /// <summary>True if ConnectionString is configured for current environment.</summary>
    public bool HasDatabase => !string.IsNullOrEmpty(Settings?.ConnectionString);

    /// <summary>True if DomainUrl is configured for current environment.</summary>
    public bool HasDomainUrl => !string.IsNullOrEmpty(Settings?.DomainUrl);

    /// <summary>Gets the current environment settings for this project.</summary>
    protected ProjectSettings? Settings => GetSettingsService().GetProjectSettings(Name);

    /// <summary>Gets the full log directory path.</summary>
    protected string? LogDirectory => Settings?.LogDirectory;

    /// <summary>Gets the domain URL for this project.</summary>
    protected string? DomainUrl => Settings?.DomainUrl;

    /// <summary>Gets or creates the data access service for this project.</summary>
    protected IDataAccessService DataAccess
    {
        get
        {
            // Don't cache - connection string may change with environment
            if (_factory == null)
                throw new InvalidOperationException(
                    "DataAccessServiceFactory not initialized. Call ProjectBase.SetDataAccessServiceFactory() in Program.cs");

            var connectionString = Settings?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException(
                    $"Project '{Name}' does not have ConnectionString configured for current environment");

            return _factory.CreateService(connectionString);
        }
    }

    /// <summary>
    /// Set the factory once during app startup.
    /// </summary>
    public static void SetDataAccessServiceFactory(IDataAccessServiceFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Set the project settings service once during app startup.
    /// </summary>
    public static void SetProjectSettingsService(IProjectSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

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

    /// <summary>
    /// Gets the settings service, throwing if not initialized.
    /// </summary>
    private static IProjectSettingsService GetSettingsService()
    {
        return _settingsService ?? throw new InvalidOperationException(
            "ProjectSettingsService not initialized. Call ProjectBase.SetProjectSettingsService() in Program.cs");
    }
}
