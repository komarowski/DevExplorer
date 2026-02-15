namespace DevExplorer.Domain.Models;

/// <summary>
/// Configuration options for environment-specific settings.
/// </summary>
public class EnvironmentSettingsOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "EnvironmentSettings";

    /// <summary>The default environment name (can be changed at runtime).</summary>
    public string DefaultEnvironment { get; set; } = "Develop";

    /// <summary>Environment-specific configurations.</summary>
    public Dictionary<string, EnvironmentSettings> Environments { get; set; } = [];
}
