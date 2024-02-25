using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullMC.APM.Internal.Processors;
using System.CommandLine;

namespace NullMC.APM.Internal.Commands;

internal class BlockCommand : Command
{
    internal static BlockCommand Instance {get;} = new(null);

    private readonly IServiceProvider? provider;


    public BlockCommand(IServiceProvider? provider)
        : base("block", "Generates a blocks.glsl file defining block ID's using comments from block.properties.")
    {
        this.provider = provider;

        var argPropertiesFile = new Argument<FileInfo>("properties", () => new FileInfo("block.properties"), "The block.properties file to scan. This file is used for writing if a template is specified.");
        Add(argPropertiesFile);

        var argDefinesFile = new Argument<FileInfo>("defines", () => new FileInfo("blocks.glsl"), "The blocks.glsl file to generate.");
        Add(argDefinesFile);

        var optPropertiesTemplateFile = new Option<FileInfo>(["-t", "template"], "An optional template block.properties file to scan.");
        Add(optPropertiesTemplateFile);

        this.SetHandler(RunAsync, argPropertiesFile, argDefinesFile, optPropertiesTemplateFile);
    }

    private async Task RunAsync(FileInfo propertiesFile, FileInfo defineFile, FileInfo? templatePropertiesFile)
    {
        ArgumentNullException.ThrowIfNull(provider);
        var logger = provider.GetRequiredService<ILogger<BlockCommand>>();
        var processor = provider.GetRequiredService<BlockPropertiesProcessor>();

        logger.LogDebug("Generating block block-defines from file [{FullName}]...", propertiesFile.FullName);

        try {
            await processor.ProcessAsync(propertiesFile.FullName, defineFile.FullName, templatePropertiesFile?.FullName);

            logger.LogInformation("Successfully generated block-defines for file [{FullName}]...", propertiesFile.FullName);
        }
        catch (Exception error) {
            logger.LogError(error, "Failed to generate block-defines for file [{FullName}]!", propertiesFile.FullName);
        }
    }
}
