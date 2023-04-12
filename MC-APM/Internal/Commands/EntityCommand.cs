using MC_ABP.Internal.Processors;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace MC_ABP.Internal.Commands;

internal class EntityCommand : Command
{
    private readonly ILogger<EntityCommand> logger;
    private readonly EntityPropertiesProcessor processor;


    public EntityCommand(
        ILogger<EntityCommand> logger,
        EntityPropertiesProcessor processor)
            : base("entity", "Generates an entities.glsl file defining entity ID's using comments from entity.properties.")
    {
        this.processor = processor;
        this.logger = logger;

        var argPropertiesFile = new Argument<FileInfo>("properties", () => new FileInfo("entity.properties"), "The entity.properties file to scan. This file is used for writing if a template is specified.");
        Add(argPropertiesFile);

        var argDefinesFile = new Argument<FileInfo>("defines", () => new FileInfo("entities.glsl"), "The entities.glsl file to generate.");
        Add(argDefinesFile);

        var optPropertiesTemplateFile = new Option<FileInfo>(new []{"-t", "template"}, "An optional template entity.properties file to scan.");
        Add(optPropertiesTemplateFile);

        this.SetHandler(RunAsync, argPropertiesFile, argDefinesFile, optPropertiesTemplateFile);
    }

    private async Task RunAsync(FileInfo propertiesFile, FileInfo defineFile, FileInfo? templatePropertiesFile)
    {
        logger.LogDebug("Generating entity-defines from file [{FullName}]...", propertiesFile.FullName);

        try {
            await processor.ProcessAsync(propertiesFile.FullName, defineFile.FullName, templatePropertiesFile?.FullName);

            logger.LogDebug("Successfully generated entity-defines for file [{FullName}]...", propertiesFile.FullName);
        }
        catch (Exception error) {
            logger.LogError(error, "Failed to generate entity-defines for file [{FullName}]!", propertiesFile.FullName);
        }
    }
}
