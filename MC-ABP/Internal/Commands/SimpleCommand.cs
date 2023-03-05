using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace MC_ABP.Internal.Commands;

internal class SimpleCommand : Command
{
    private readonly ILogger<SimpleCommand> logger;
    private readonly ISimpleDefineGenerator generator;


    public SimpleCommand(
        ILogger<SimpleCommand> logger,
        ISimpleDefineGenerator generator)
            : base("simple", "Generates a blocks.glsl file defining block ID's using comments from block.properties.")
    {
        this.generator = generator;
        this.logger = logger;

        var argPropertiesFile = new Argument<FileInfo>("properties", "The block.properties file to scan. This file is used for writing if a template is specified.");
        Add(argPropertiesFile);

        var argDefinesFile = new Argument<FileInfo>("defines", "The blocks.glsl file to generate.");
        Add(argDefinesFile);

        var optPropertiesTemplateFile = new Option<FileInfo>(new []{"-t", "template"}, "An optional template block.properties file to scan.");
        Add(optPropertiesTemplateFile);

        this.SetHandler(RunAsync, argPropertiesFile, argDefinesFile, optPropertiesTemplateFile);
    }

    private async Task RunAsync(FileInfo propertiesFile, FileInfo defineFile, FileInfo? templatePropertiesFile)
    {
        logger.LogDebug("Generating simple block-defines from file [{FullName}]...", propertiesFile.FullName);

        try {
            await generator.GenerateAsync(propertiesFile.FullName, defineFile.FullName, templatePropertiesFile?.FullName);

            logger.LogDebug("Successfully generated simple-defines for file [{FullName}]...", propertiesFile.FullName);
        }
        catch (Exception error) {
            logger.LogError(error, "Failed to generate simple-defines for file [{FullName}]!", propertiesFile.FullName);
        }
    }
}
