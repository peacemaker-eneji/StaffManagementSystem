using Microsoft.AspNetCore.Identity;
using StaffManagementSystem.Domain.Enums;
using System.Text.Json.Serialization;

namespace StaffManagementSystem.Domain.Models {
    public class User : IdentityUser {
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public bool IsActive { get; set; } = true;
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public IEnumerable<AttendanceRecord>? AttendanceRecords;
    }
}

