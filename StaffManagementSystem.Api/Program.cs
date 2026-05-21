// Builder Config
var builder = WebApplication.CreateBuilder(args);
var bconfig = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddCors(options => options.AddPolicy("AllowSpecificOrigin", policy =>
            policy.WithOrigins("https://localhost:7140")
                  .AllowAnyHeader()
                  .AllowAnyMethod()));





//// APP Confing
var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.MapControllers();
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();