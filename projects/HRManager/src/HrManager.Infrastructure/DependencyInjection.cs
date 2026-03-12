using Azure.Storage.Blobs;
using HrManager.Application.Common.Interfaces;
using HrManager.Application.Common.Services;
using HrManager.Infrastructure.Persistance;
using HrManager.Infrastructure.Persistance.Configurations.Settings;
using HrManager.Infrastructure.Persistance.Interceptors;
using HrManager.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace HrManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        options.UseNpgsql(builder.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>()));

        services.Configure<CorsSettings>(configuration.GetSection("Cors"));

        var azureBlobSettings = new AzureBlobSettings(
            configuration["AzureBlobSettings:ConnectionString"]!,
            configuration["AzureBlobSettings:ContainerName"]!);
        services.AddSingleton(azureBlobSettings);

        services.AddSingleton<BlobServiceClient>(_ =>
            new BlobServiceClient(azureBlobSettings.ConnectionString));

        services.AddSingleton(sp =>
        {
            var blobService = sp.GetRequiredService<BlobServiceClient>();
            return blobService.GetBlobContainerClient(azureBlobSettings.ContainerName);
        });

        services.AddScoped<IStorageService, AzureBlobStorageService>();

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTimeService, DateTimeService>();

        services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();

        return services;
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
}
