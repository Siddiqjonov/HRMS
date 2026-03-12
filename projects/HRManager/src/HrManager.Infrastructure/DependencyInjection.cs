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

namespace HrManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        options.UseSqlServer(connectionString)
            .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>()));

        services.Configure<CorsSettings>(configuration.GetSection("Cors"));

        var firebaseStorageSettings = new FirebaseStorageSettings(
            configuration["FirebaseStorage:BucketName"]!,
            configuration["FirebaseStorage:ServiceAccountKeyPath"] ?? string.Empty,
            configuration["FirebaseStorage:ServiceAccountKeyJson"]);
        services.AddSingleton(firebaseStorageSettings);

        services.AddScoped<IStorageService, FirebaseStorageService>();

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
