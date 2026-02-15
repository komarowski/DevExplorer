using DevExplorer.Domain.Contracts;
using DevExplorer.Domain.Models;
using Microsoft.Extensions.Options;

namespace DevExplorer.Infrastructure.Services;

/// <summary>
/// Service for accessing environment-specific project settings from configuration.
/// </summary>
public class ProjectSettingsService : IProjectSettingsService
{
    private readonly EnvironmentSettingsOptions _options;
    private readonly Dictionary<string, Dictionary<string, ProjectSettings>> _settingsCache = new(StringComparer.OrdinalIgnoreCase);
    private string _currentEnvironment;

    public ProjectSettingsService(IOptions<EnvironmentSettingsOptions> options)
    {
        _options = options.Value;
        _currentEnvironment = _options.DefaultEnvironment;
        BuildAllCaches();
    }

    /// <inheritdoc />
    public string CurrentEnvironment
    {
        get => _currentEnvironment;
        set
        {
            if (!_currentEnvironment.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                _currentEnvironment = value;
                EnvironmentChanged?.Invoke(value);
            }
        }
    }

    /// <inheritdoc />
    public event Action<string>? EnvironmentChanged;

    /// <inheritdoc />
    public IReadOnlyList<string> AvailableEnvironments =>
        _options.Environments.Keys.ToList().AsReadOnly();

    /// <inheritdoc />
    public ProjectSettings? GetProjectSettings(string projectName)
    {
        if (!_settingsCache.TryGetValue(_currentEnvironment, out var envSettings))
            return null;

        return envSettings.GetValueOrDefault(projectName.ToLowerInvariant());
    }

    /// <inheritdoc />
    public IReadOnlyList<ProjectSettings> GetAllProjectSettings()
    {
        if (!_settingsCache.TryGetValue(_currentEnvironment, out var envSettings))
            return [];

        return envSettings.Values.ToList().AsReadOnly();
    }

    private void BuildAllCaches()
    {
        foreach (var (envKey, envConfig) in _options.Environments)
        {
            _settingsCache[envKey] = envConfig.Projects.ToDictionary(
                p => p.Name.ToLowerInvariant(),
                p => p);
        }
    }
}
