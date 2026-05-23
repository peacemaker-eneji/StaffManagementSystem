using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Domain.Interfaces {
    public interface IAppDbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    }
}
