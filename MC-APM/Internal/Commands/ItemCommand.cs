using System.CommandLine;
using Microsoft.Extensions.Logging;
using NullMC.APM.Internal.Processors;

namespace NullMC.APM.Internal.Commands;

internal class ItemCommand : Command
{
    private readonly ILogger<ItemCommand> logger;
    private readonly ItemPropertiesProcessor processor;


    public ItemCommand(
        ILogger<ItemCommand> logger,
        ItemPropertiesProcessor processor)
            : base("item", "Generates an items.glsl file defining item ID's using comments from item.properties.")
    {
        this.processor = processor;
        this.logger = logger;

        var argPropertiesFile = new Argument<FileInfo>("properties", () => new FileInfo("item.properties"), "The item.properties file to scan. This file is used for writing if a template is specified.");
        Add(argPropertiesFile);

        var argDefinesFile = new Argument<FileInfo>("defines", () => new FileInfo("items.glsl"), "The items.glsl file to generate.");
        Add(argDefinesFile);

        var optPropertiesTemplateFile = new Option<FileInfo>(new []{"-t", "template"}, "An optional template item.properties file to scan.");
        Add(optPropertiesTemplateFile);

        this.SetHandler(RunAsync, argPropertiesFile, argDefinesFile, optPropertiesTemplateFile);
    }

    private async Task RunAsync(FileInfo propertiesFile, FileInfo defineFile, FileInfo? templatePropertiesFile)
    {
        logger.LogDebug("Generating item-defines from file [{FullName}]...", propertiesFile.FullName);

        try {
            await processor.ProcessAsync(propertiesFile.FullName, defineFile.FullName, templatePropertiesFile?.FullName);

            logger.LogDebug("Successfully generated item-defines for file [{FullName}]...", propertiesFile.FullName);
        }
        catch (Exception error) {
            logger.LogError(error, "Failed to generate item-defines for file [{FullName}]!", propertiesFile.FullName);
        }
    }
}
