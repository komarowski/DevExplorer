namespace DevExplorer.Domain.Models;

/// <summary>
/// Query filter for retrieving logs.
/// </summary>
public class LogFilter
{
    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }
}