using StaffManagementSystem.Domain.Enums;
using System.Text.Json.Serialization;

namespace StaffManagementSystem.Domain.Models {
    public class AttendanceRecord {
        public string Id { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? ClockIn { get; set; }
        public TimeOnly? ClockOut { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttendanceStatus Status { get; set; }
    }
}
