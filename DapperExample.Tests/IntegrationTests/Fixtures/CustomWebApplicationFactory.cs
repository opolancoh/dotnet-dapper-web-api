using DapperExample.Tests.Helpers;
using DapperExample.Web.Data.DatabaseContext;
using DapperExample.Web.Migrations;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DapperExample.Tests.IntegrationTests.Fixtures;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Override application configuration
        builder.ConfigureAppConfiguration(config =>
        {
            config.Sources.Clear();

            var newConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            config.AddConfiguration(newConfig);
        });

        // Override application services
        builder.ConfigureServices(services =>
        {
            var stringConnection =
                "Server=localhost;Database=books_dapper_db_test;Username=postgres;Password=My@Passw0rd;";
            // Remove and add services
            // DbContext
            var dbContextService = services.SingleOrDefault(x => x.ServiceType == typeof(DapperContext));
            if (dbContextService != null) services.Remove(dbContextService);
            services.AddSingleton(x => new DapperContext(stringConnection));

            // Db Migrator
            services
                // Logging is the replacement for the old IAnnouncer
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Registration of all FluentMigrator-specific services
                .AddFluentMigratorCore()
                // Configure the runner
                .ConfigureRunner(
                    runnerBuilder => runnerBuilder
                        // Use target DB
                        .AddPostgres()
                        // The target DB connection string
                        .WithGlobalConnectionString(stringConnection)
                        // Specify the assembly with the migrations
                        .ScanIn(typeof(AddTablesMigration).Assembly));

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

            // Create Database
            var db = scopedServices.GetRequiredService<DapperContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Create Tables
            var dbMigration = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            dbMigration.ListMigrations();
            dbMigration.MigrateUp();

            // Add Data
            // Don't update/remove this initial data
            var books = DbDataHelper.Books;
            db.AddBooks(books);

            var reviews = DbDataHelper.Reviews;
            db.AddReviews(reviews);

            logger.LogInformation("All data was saved successfully");
        });
    }
}