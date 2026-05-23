// Builder Config
using DotNetEnv;
using StaffManagementSystem.Api;
using StaffManagementSystem.Application;
using StaffManagementSystem.Infrastructure;
using StaffManagementSystem.Infrastructure.Persistence;

Env.Load();


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddApplication();
builder.Services.AddInfrastructure(config);
builder.Services.AddPresentation();

//// APP Confing
var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
await DatabaseInitializer.InitializeAsync(app.Services);

app.Run();