using DapperExample.Tests.Helpers;
using DapperExample.Web.Contracts;
using DapperExample.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DapperExample.Tests.IntegrationTests.Fixtures;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove current DbContext
            var serviceDescriptor = services.SingleOrDefault(x => x.ServiceType == typeof(IDapperContext));
            if (serviceDescriptor != null) services.Remove(serviceDescriptor);

            // Add DbContext for testing
            const string connectionString =
                "Server=localhost; Database=books_dapper_db_test; Username=postgres; Password=My@Passw0rd;";
            services.AddSingleton<IDapperContext>(x => new DapperContext(connectionString));

            //
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();
            // var dbContext = scopedServices.GetRequiredService<DapperContext>();
            var dbContext = scopedServices.GetRequiredService<IDapperContext>();

            var dbHelper = new DbHelper(dbContext, "");
            // dbHelper.EnsureDeleted();
            // dbHelper.EnsureCreated();

            // Don't update/remove this initial data
            var books = DbDataHelper.Books;
            dbHelper.AddBooks(books);

            var reviews = DbDataHelper.Reviews;
            dbHelper.AddReviews(reviews);

            logger.LogError("All data was saved successfully");
        });
    }
}