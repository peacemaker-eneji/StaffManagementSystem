using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;
using StaffManagementSystem.Infrastucture;

namespace StaffManagementSystem.Infrastructure.Persistence {
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options), IAppDbContext {
        public override DbSet<User> Users { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<BulkImportJob> BulkImportJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(InfrastructureAssembly.Assembly);
        }
    }
}
