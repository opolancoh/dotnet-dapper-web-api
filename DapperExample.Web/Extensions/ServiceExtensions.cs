using DapperExample.Web.Contracts;
using DapperExample.Web.Data.DatabaseContext;
using DapperExample.Web.Migrations;
using DapperExample.Web.Services;
using FluentMigrator.Runner;

namespace DapperExample.Web.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }

    public static void ConfigurePersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IReviewService, ReviewService>();
    }

    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(x => new DapperContext(configuration.GetConnectionString("ApplicationDbConnection")));
    }

    public static void ConfigureDbMigration(this IServiceCollection services, IConfiguration configuration)
    {
        services
            // Logging is the replacement for the old IAnnouncer
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Registration of all FluentMigrator-specific services
            .AddFluentMigratorCore()
            // Configure the runner
            .ConfigureRunner(
                builder => builder
                    // Use target DB
                    .AddPostgres()
                    // The target DB connection string
                    .WithGlobalConnectionString(configuration.GetConnectionString("ApplicationDbConnection"))
                    // Specify the assembly with the migrations
                    .ScanIn(typeof(AddTablesMigration).Assembly))
            .BuildServiceProvider();
    }
}