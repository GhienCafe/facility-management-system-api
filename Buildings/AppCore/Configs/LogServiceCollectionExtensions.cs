using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace AppCore.Configs;

public static class LogServiceCollectionExtensions
{
    public static IServiceCollection AddLogging(this IServiceCollection services, string logFolder = "Logs")
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Default", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Fatal)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Update", LogEventLevel.Fatal)
            .WriteTo.Console()
            .WriteTo.File($"{logFolder}/log_.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        services.AddSingleton(Log.Logger);
        return services;
    }
}