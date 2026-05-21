using Microsoft.AspNetCore.Identity;

namespace StaffManagementSystem.Api.Extensions {
    public static class DbSetupExtension {
        public static WebApplication UseDbSetup(this WebApplication app) {
            //using var scope = app.Services.CreateScope();
            //scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();

            //var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //var roles = new[] { "Admin", "User" };
            //foreach (var role in roles) {
            //    if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
            //}
            return app;
        }
    }
}
