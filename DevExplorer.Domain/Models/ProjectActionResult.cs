namespace DevExplorer.Domain.Models;

/// <summary>
/// Result of executing a project action.
/// </summary>
public record ProjectActionResult(bool Success, string? Data, string? ContentType = null, string? Message = null);
