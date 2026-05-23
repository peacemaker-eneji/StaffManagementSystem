using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Infrastructure.Persistence.Configurations {
    public class UserConfiguration : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            builder.Property(u => u.Role)
                .HasConversion<string>();
        }
    }
}
