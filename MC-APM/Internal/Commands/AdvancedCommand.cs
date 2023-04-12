using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace NullMC.APM.Internal.Commands;

internal class AdvancedCommand : Command
{
    private readonly ILogger<AdvancedCommand> logger;
    private readonly IAdvancedDefineGenerator generator;


    public AdvancedCommand(
        ILogger<AdvancedCommand> logger,
        IAdvancedDefineGenerator generator)
            : base("advanced", "Generates a blocks.glsl file defining block ID's using comments from block.properties.")
    {
        this.generator = generator;
        this.logger = logger;

        var argJsonBlocksFile = new Argument<FileInfo>("json", "The blocks.json file to scan.");
        Add(argJsonBlocksFile);

        var argPropertiesFile = new Argument<FileInfo>("properties", "The block.properties file to generate.");
        Add(argPropertiesFile);

        var argDefinesFile = new Argument<FileInfo>("defines", "The blocks.glsl file to generate.");
        Add(argDefinesFile);

        this.SetHandler(RunAsync, argJsonBlocksFile, argPropertiesFile, argDefinesFile);
    }

    private async Task RunAsync(FileInfo jsonBlocksFile, FileInfo propertiesFile, FileInfo defineFile)
    {
        logger.LogDebug("Generating advanced block-defines from file [{FullName}]...", jsonBlocksFile.FullName);

        try {
            await generator.GenerateAsync(jsonBlocksFile.FullName, propertiesFile.FullName, defineFile.FullName);

            logger.LogDebug("Successfully generated advanced block-defines for file [{FullName}]...", jsonBlocksFile.FullName);
        }
        catch (Exception error) {
            logger.LogError(error, "Failed to generate advanced block-defines for file [{FullName}]!", jsonBlocksFile.FullName);
        }
    }
}
