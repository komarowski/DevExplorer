namespace DevExplorer.Domain.Models
{
    /// <summary>
    /// Configuration for a specific environment.
    /// </summary>
    public class EnvironmentSettings
    {
        /// <summary>Project-specific settings for this environment.</summary>
        public List<ProjectSettings> Projects { get; set; } = [];
    }
}
