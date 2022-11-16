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
            // Remove and add services
            // DbContext
            var dbContextService = services.SingleOrDefault(x => x.ServiceType == typeof(DapperContext));
            if (dbContextService != null) services.Remove(dbContextService);
            services.AddSingleton<DapperContext>();

            // Db Migrator
            var dbMigrationService = services.SingleOrDefault(x => x.ServiceType == typeof(IMigrationRunner));
            if (dbMigrationService != null) services.Remove(dbMigrationService);
            services
                .AddLogging(x => x.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c
                    .AddPostgres()
                    .WithGlobalConnectionString("Server=localhost;Database=books_dapper_db_test;Username=postgres;Password=My@Passw0rd;")
                    .WithMigrationsIn(typeof(AddTablesMigration).Assembly));
            
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

            logger.LogError("All data was saved successfully");
        });
    }
}