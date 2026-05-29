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

        private static TimeOnly ResumptionTime = new TimeOnly(11); // 11 am
        private static TimeOnly GraceEnd = new TimeOnly(11, 20);
        private static TimeOnly HalfDayCutoff = ResumptionTime.Add(new TimeSpan(4, 30, 0));

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

        public void CheckOut(TimeOnly clockOutTime) {
            if (ClockIn is null)
                throw new DomainException("Cannot clock out without clocking in first.");

            if (clockOutTime <= ClockIn)
                throw new DomainException("Clock-out time must be after clock-in time.");

            if (ClockOut is not null)
                throw new DomainException("Already clocked out for today.");

            ClockOut = clockOutTime;
            if ((ClockOut.Value - ClockIn.Value).TotalHours < 4) IsHalfDay = true;
        }
    }
}
