using System.CommandLine;
using Microsoft.Extensions.Logging;
using NullMC.APM.Internal.Processors;

namespace NullMC.APM.Internal.Commands;

internal class BlockCommand : Command
{
    private readonly ILogger<BlockCommand> logger;
    private readonly BlockPropertiesProcessor processor;


    public BlockCommand(
        ILogger<BlockCommand> logger,
        BlockPropertiesProcessor processor)
            : base("block", "Generates a blocks.glsl file defining block ID's using comments from block.properties.")
    {
        this.processor = processor;
        this.logger = logger;

        var argPropertiesFile = new Argument<FileInfo>("properties", () => new FileInfo("block.properties"), "The block.properties file to scan. This file is used for writing if a template is specified.");
        Add(argPropertiesFile);

        var argDefinesFile = new Argument<FileInfo>("defines", () => new FileInfo("blocks.glsl"), "The blocks.glsl file to generate.");
        Add(argDefinesFile);

        var optPropertiesTemplateFile = new Option<FileInfo>(new []{"-t", "template"}, "An optional template block.properties file to scan.");
        Add(optPropertiesTemplateFile);

        this.SetHandler(RunAsync, argPropertiesFile, argDefinesFile, optPropertiesTemplateFile);
    }

    private async Task RunAsync(FileInfo propertiesFile, FileInfo defineFile, FileInfo? templatePropertiesFile)
    {
        logger.LogDebug("Generating block block-defines from file [{FullName}]...", propertiesFile.FullName);

        try {
            await processor.ProcessAsync(propertiesFile.FullName, defineFile.FullName, templatePropertiesFile?.FullName);

            logger.LogDebug("Successfully generated block-defines for file [{FullName}]...", propertiesFile.FullName);
        }
        catch (Exception error) {
            logger.LogError(error, "Failed to generate block-defines for file [{FullName}]!", propertiesFile.FullName);
        }
    }
}
