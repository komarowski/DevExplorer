namespace DevExplorer.Domain.Contracts;

/// <summary>
/// Registry contract for discovering projects.
/// </summary>
public interface IProjectRegistry
{
    /// <summary>Get all registered projects for a specific project name.</summary>
    IReadOnlyList<IProject> GetProject(string name);

    /// <summary>Get all registered projects.</summary>
    IReadOnlyList<IProject> GetAllProjects();
}
