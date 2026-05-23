using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StaffManagementSystem.Infrastructure.Persistence.Seeders;

namespace StaffManagementSystem.Infrastructure.Persistence {
    public static class DatabaseInitializer {
        public static async Task InitializeAsync(IServiceProvider serviceProvider) {
            await MigrateAsync(serviceProvider);
            await DatabaseSeeder.SeedAsync(serviceProvider);
        }
        public static async Task MigrateAsync(IServiceProvider serviceProvider) {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();
        }
    }
}
