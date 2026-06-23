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
        public bool IsHalfDay { get; set; } = false;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttendanceStatus Status { get; set; }

        public static TimeOnly ResumptionTime = new TimeOnly(11, 0); // 11 am
        public static TimeOnly GraceEnd = new TimeOnly(11, 15);
        public static TimeOnly CloseTime = new TimeOnly(20, 0); // 8 pm

        public static AttendanceRecord CheckIn(string userId, DateOnly date, TimeOnly clockInTime) {
            var record = new AttendanceRecord {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Date = date,
                ClockIn = clockInTime
            };
            record.Status = clockInTime > GraceEnd ? AttendanceStatus.Late : AttendanceStatus.Present;
            return record;
        }

        public void CheckOut(TimeOnly checkOutTime) {
            if (ClockIn is null)
                throw new DomainException("Cannot check out without clocking in first.");

            if (checkOutTime <= ClockIn)
                throw new DomainException("Check-out time must be after check-in time.");

            if (ClockOut is not null)
                throw new DomainException("Already Checked out for today.");

            ClockOut = checkOutTime;
            if ((ClockOut.Value - ClockIn.Value).TotalHours < 4) IsHalfDay = true;
        }
    }
}
