using MC_ABP.Internal;
using MC_ABP.Internal.Commands;
using MC_ABP.Internal.Parsers;
using MC_ABP.Internal.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.CommandLine;

namespace MC_ABP;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
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
            Log.CloseAndFlush();
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
