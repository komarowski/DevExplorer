using DevExplorer.Domain.Constants;

namespace DevExplorer.Domain.Models;

/// <summary>
/// Metadata for a project action.
/// </summary>
public record ProjectAction(string Name, ProjectActionType ActionType, string Description = "");
