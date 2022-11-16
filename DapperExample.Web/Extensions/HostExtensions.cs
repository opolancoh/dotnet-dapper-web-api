using DapperExample.Web.Data;
using DapperExample.Web.Data.DatabaseContext;
using FluentMigrator.Runner;

namespace DapperExample.Web.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DapperContext>();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        try
        {
            var isCreated = db.Database.EnsureCreated();
            if (isCreated)
            {
                migrationService.ListMigrations();
                migrationService.MigrateUp();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }

        return host;
    }
}