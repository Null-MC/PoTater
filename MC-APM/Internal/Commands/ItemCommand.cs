using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullMC.APM.Internal.Processors;
using System.CommandLine;

namespace NullMC.APM.Internal.Commands;

internal class ItemCommand : Command
{
    internal static ItemCommand Instance {get;} = new(null);

    private readonly IServiceProvider? provider;


    public ItemCommand(IServiceProvider? provider)
        : base("item", "Generates an items.glsl file defining item ID's using comments from item.properties.")
    {
        this.provider = provider;

        var argPropertiesFile = new Argument<FileInfo>("properties", () => new FileInfo("item.properties"), "The item.properties file to scan. This file is used for writing if a template is specified.");
        Add(argPropertiesFile);

        var argDefinesFile = new Argument<FileInfo>("defines", () => new FileInfo("items.glsl"), "The items.glsl file to generate.");
        Add(argDefinesFile);

        var optPropertiesTemplateFile = new Option<FileInfo>(["-t", "template"], "An optional template item.properties file to scan.");
        Add(optPropertiesTemplateFile);

        this.SetHandler(RunAsync, argPropertiesFile, argDefinesFile, optPropertiesTemplateFile);
    }

    private async Task RunAsync(FileInfo propertiesFile, FileInfo defineFile, FileInfo? templatePropertiesFile)
    {
        ArgumentNullException.ThrowIfNull(provider);
        var logger = provider.GetRequiredService<ILogger<ItemCommand>>();
        var processor = provider.GetRequiredService<ItemPropertiesProcessor>();

        logger.LogDebug("Generating item-defines from file [{FullName}]...", propertiesFile.FullName);

        try {
            await processor.ProcessAsync(propertiesFile.FullName, defineFile.FullName, templatePropertiesFile?.FullName);

            logger.LogInformation("Successfully generated item-defines for file [{FullName}]...", propertiesFile.FullName);
        }
        catch (Exception error) {
            logger.LogError(error, "Failed to generate item-defines for file [{FullName}]!", propertiesFile.FullName);
        }
    }
}
