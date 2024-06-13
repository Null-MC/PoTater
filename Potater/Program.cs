using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NullMC.Potater.Internal;
using NullMC.Potater.Internal.Commands;
using NullMC.Potater.Internal.Parsers;
using NullMC.Potater.Internal.Processors;
using Serilog;
using Serilog.Events;
using System.CommandLine;

namespace NullMC.Potater;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var enableDebug = false;
        try {
            enableDebug = AppRootCommand.Instance.Parse(args)
                .FindResultFor(AppRootCommand.DebugOption)?
                .GetValueOrDefault<bool?>() ?? false;
        }
        catch {
            // ignore pre-parsing failures
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(enableDebug ? LogEventLevel.Debug : LogEventLevel.Information)
            .WriteTo.Console()
            .CreateLogger();

        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();

        ConfigureServices(config, services);

        try {
            await using var provider = services.BuildServiceProvider();
            var rootCommand = provider.GetRequiredService<AppRootCommand>();
            return await rootCommand.InvokeAsync(args);
        }
        catch (Exception error) {
            Log.Fatal(error, "An unhandled exception has occurred!");
            return 1;
        }
        finally {
            await Log.CloseAndFlushAsync();
        }
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddLogging(builder => {
            builder.AddSerilog(dispose: true);
        });

        services.AddSingleton(configuration);
        services.AddSingleton<AppRootCommand>();
        services.AddSingleton<BlockCommand>();
        services.AddSingleton<ItemCommand>();
        services.AddSingleton<EntityCommand>();
        services.AddSingleton<AdvancedCommand>();

        services.AddTransient<BlockPropertiesParser>();
        services.AddTransient<ItemPropertiesParser>();
        services.AddTransient<EntityPropertiesParser>();

        services.AddTransient<BlockPropertiesProcessor>();
        services.AddTransient<ItemPropertiesProcessor>();
        services.AddTransient<EntityPropertiesProcessor>();

        services.AddTransient<IAdvancedDefineGenerator, AdvancedDefineGenerator>();
    }
}
