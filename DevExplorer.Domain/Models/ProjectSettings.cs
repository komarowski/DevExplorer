namespace DevExplorer.Domain.Models;

/// <summary>
/// Environment-specific settings for a project (DTO from JSON config).
/// </summary>
public class ProjectSettings
{
    /// <summary>Project name (must match ProjectAttribute name).</summary>
    public required string Name { get; set; }

    /// <summary>User-friendly display name for UI.</summary>
    public string? DisplayName { get; set; }

    /// <summary>Root directory for log files.</summary>
    public string? LogDirectory { get; set; }

    /// <summary>SQL Server connection string.</summary>
    public string? ConnectionString { get; set; }

    /// <summary>Domain URL for the project.</summary>
    public string? DomainUrl { get; set; }

    /// <summary>Additional flexible settings accessible via indexer.</summary>
    public Dictionary<string, string> Settings { get; set; } = [];

    /// <summary>Gets a custom setting value by key.</summary>
    public string? this[string key] => Settings.GetValueOrDefault(key);
}
