using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace BlogPostService.Infrastructure.Npgsql.Extensions;

public static class HostServiceExtensions
{
    public static IHost MigrateUp(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger>();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        try
        {
            logger.LogInformation("Starting database migration...");
            runner.MigrateUp();
        }
        catch (Exception exception)
        {
            logger.LogError($"Migration failed: {exception.Message}");
            throw;
        }

        return host;
    }
}