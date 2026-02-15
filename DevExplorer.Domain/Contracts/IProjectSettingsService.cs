using DevExplorer.Domain.Models;

namespace DevExplorer.Domain.Contracts;

/// <summary>
/// Service for accessing environment-specific project settings.
/// Supports runtime environment switching.
/// </summary>
public interface IProjectSettingsService
{
    /// <summary>Gets or sets the current environment name. Can be changed at runtime.</summary>
    string CurrentEnvironment { get; set; }

    /// <summary>Event raised when environment changes.</summary>
    event Action<string>? EnvironmentChanged;

    /// <summary>Gets available environment names (from appsettings.json).</summary>
    IReadOnlyList<string> AvailableEnvironments { get; }

    /// <summary>Gets settings for a project in the current environment.</summary>
    ProjectSettings? GetProjectSettings(string projectName);

    /// <summary>Gets all configured projects for the current environment.</summary>
    IReadOnlyList<ProjectSettings> GetAllProjectSettings();
}
