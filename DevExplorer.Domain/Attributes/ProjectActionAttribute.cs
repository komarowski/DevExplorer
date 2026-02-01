using DevExplorer.Domain.Constants;

namespace DevExplorer.Domain.Attributes;

/// <summary>
/// Marks a method as a discoverable project action.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ProjectActionAttribute(ProjectActionType actionType, string description = "") : Attribute
{
    public ProjectActionType ActionType { get; } = actionType;

    public string Description { get; } = description;
}
