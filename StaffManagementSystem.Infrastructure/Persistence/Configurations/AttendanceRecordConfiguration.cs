using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Infrastructure.Persistence.Configurations {
    public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord> {
        public void Configure(EntityTypeBuilder<AttendanceRecord> builder) {
            builder.HasOne(r => r.User)
                .WithMany(u => u.AttendanceRecords)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => new { r.UserId, r.Date })
                .IsUnique();

            builder.Property(u => u.Status)
                .HasConversion<string>();
        }
    }
}
