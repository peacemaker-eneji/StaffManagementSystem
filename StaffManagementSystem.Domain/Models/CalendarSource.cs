namespace StaffManagementSystem.Domain.Models;

/// <summary>
/// A configured ICS calendar feed (e.g. "Public Holidays - Nigeria") that Hangfire
/// periodically fetches and parses.
/// </summary>
public class CalendarSource {
    public string Id { get; set; }
    public string Name { get; set; }
    public string IcsUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromHours(6);
}
