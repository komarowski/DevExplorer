namespace DevExplorer.Domain.Attributes;

/// <summary>
/// Marks a class as a project and defines its metadata.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ProjectAttribute(string name) : Attribute
{
    /// <summary>Unique identifier for the project.</summary>
    public string Name { get; } = name;
}
