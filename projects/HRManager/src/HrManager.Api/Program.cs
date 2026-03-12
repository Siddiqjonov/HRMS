using HrManager.Api;
using HrManager.Api.Infrastructure;
using HrManager.Application;
using HrManager.Infrastructure;
using HrManager.Infrastructure.Persistance.Configurations.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.Host.UseSerilog((context, configuration)
    => configuration.ReadFrom.Configuration(context.Configuration));

builder.AddWebServices();

builder.Services.AddHealthChecks();

builder.Services.AddProblemDetails();

var corsSettings = builder.Configuration.GetSection("Cors").Get<CorsSettings>();

builder.Services
    .AddCors(options =>
    {
        options.AddPolicy("AllowOrigin", builder =>
            builder.WithOrigins(corsSettings!.AllowedOrigins)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials());
    });

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) 
{
    app.ApplyMigrations();
}

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/health");

app.UseExceptionHandler();

app.UseSerilogRequestLogging();

app.UseCors("AllowOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

await app.RunAsync();