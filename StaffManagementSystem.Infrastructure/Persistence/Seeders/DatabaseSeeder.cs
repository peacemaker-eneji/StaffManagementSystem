using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Infrastructure.Persistence.Seeders {
    static public class DatabaseSeeder {
        static public async Task SeedAsync(IServiceProvider serviceProvider) {
            await SeedRolesAsync(serviceProvider);
            await SeedUsersAsync(serviceProvider);
            await SeedCalendarSourceAsync(serviceProvider);
        }
        static public async Task SeedRolesAsync(IServiceProvider serviceProvider) {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = Enum.GetNames(typeof(Role));
            foreach (string role in roles) {
                if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        static public async Task SeedUsersAsync(IServiceProvider serviceProvider) {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string[][] names = [
                ["Chukwuemeka", "Ajayi"],
                ["Ebele", "Nzekwe"],
                ["Dele", "Osei"],
                ["Adaobiageli", "Madu"],
                ["Emeka", "Ugwu"]
            ];

            List<User> users = [new User { Email = "admin@gmail.com", Firstname = "Eneji", Lastname = "Ohieku", Role = Role.Admin }];

            foreach (var name in names) {
                users.Add(new User {
                    Firstname = name[0],
                    Lastname = name[1],
                    Email = $"{name[0]}.{name[1]}@gmail.com",
                    Role = Role.Employee
                });
            }

            foreach (var user in users) {
                user.UserName = user.Email;
                if (await userManager.FindByEmailAsync(user.Email) is not null) continue;
                await userManager.CreateAsync(user, Environment.GetEnvironmentVariable("DEFAULT_PASSWORD")!);
                await userManager.AddToRoleAsync(user, user.Role.ToString());
            }
        }
        static public async Task SeedCalendarSourceAsync(IServiceProvider serviceProvider) {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            List<CalendarSource> calendars = [
                new CalendarSource{
                    Name = "Nigerian Holiday Calendar",
                    IcsUrl = "https://www.officeholidays.com/ics-all/nigeria"
                }
            ];
            
            foreach (CalendarSource cal in calendars) {
                if (await context.CalendarSources.FirstOrDefaultAsync(s => s.IcsUrl == cal.IcsUrl) is not null) continue;
                cal.Id = Guid.NewGuid().ToString();
                await context.CalendarSources.AddAsync(cal);
            }
            await context.SaveChangesAsync();
        }
    }
}
