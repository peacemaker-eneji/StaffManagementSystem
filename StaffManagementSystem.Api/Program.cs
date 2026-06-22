// Builder Config
using DotNetEnv;
using Hangfire;
using StaffManagementSystem.Api;
using StaffManagementSystem.Application;
using StaffManagementSystem.Infrastructure;
using StaffManagementSystem.Infrastructure.Jobs;
using StaffManagementSystem.Infrastructure.Persistence;

Env.Load();


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Logging.AddConsole();
builder.Services.AddApplication(config);
builder.Services.AddInfrastructure(config);
builder.Services.AddPresentation(config);

//// APP Confing
var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireServer();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}
await DatabaseInitializer.InitializeAsync(app.Services);
JobScheduler.RegisterRecurringJobs();

app.Run();