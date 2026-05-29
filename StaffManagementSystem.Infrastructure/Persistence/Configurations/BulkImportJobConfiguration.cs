using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Infrastructure.Persistence.Configurations {
    public class BulkImportJobConfiguration : IEntityTypeConfiguration<BulkImportJob> {
        public void Configure(EntityTypeBuilder<BulkImportJob> builder) {
            builder.Property(u => u.Status)
                .HasConversion<string>();
        }
    }
}
