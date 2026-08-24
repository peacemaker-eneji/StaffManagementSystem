namespace StaffManagementSystem.Domain.Models;

/// <summary>
/// A single holiday/event occurrence, already expanded from any recurrence rule.
/// This is what gets cached and what the AI tool reads from.
/// </summary>
public class Holiday {
    public string Id { get; set; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public bool IsAllDay { get; init; }
    public string? Location { get; init; }
    public string SourceCalendarId { get; init; }
}
