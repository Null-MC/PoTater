using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullMC.APM.Internal.Parsers;
using NullMC.APM.Internal.Processors;
using Xunit.Abstractions;

namespace NullMC.APM.Tests.Internal;

public abstract class TestBase : IDisposable
{
    private readonly ServiceProvider provider;
    //private readonly ITestOutputHelper outputHelper;

    protected IServiceProvider Provider => provider;


    protected TestBase(ITestOutputHelper outputHelper)
    {
        //this.outputHelper = outputHelper;

        var services = new ServiceCollection();

        services.AddSingleton(outputHelper);
        services.AddSingleton(typeof(ILogger<>), typeof(TestLogger<>));
        services.AddSingleton<ILogger, TestLogger>();

        services.AddTransient<BlockPropertiesParser>();
        services.AddTransient<ItemPropertiesParser>();
        services.AddTransient<EntityPropertiesParser>();

        services.AddTransient<BlockPropertiesProcessor>();
        services.AddTransient<ItemPropertiesProcessor>();
        services.AddTransient<EntityPropertiesProcessor>();

        provider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        provider.Dispose();
    }
}
