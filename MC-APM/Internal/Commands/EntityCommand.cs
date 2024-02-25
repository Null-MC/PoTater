using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NullMC.APM.Internal.Processors;
using System.CommandLine;

namespace NullMC.APM.Internal.Commands;

internal class EntityCommand : Command
{
    internal static EntityCommand Instance {get;} = new(null);

    private readonly IServiceProvider? provider;


    public EntityCommand(IServiceProvider? provider)
        : base("entity", "Generates an entities.glsl file defining entity ID's using comments from entity.properties.")
    {
        this.provider = provider;

        var argPropertiesFile = new Argument<FileInfo>("properties", () => new FileInfo("entity.properties"), "The entity.properties file to scan. This file is used for writing if a template is specified.");
        Add(argPropertiesFile);

        var argDefinesFile = new Argument<FileInfo>("defines", () => new FileInfo("entities.glsl"), "The entities.glsl file to generate.");
        Add(argDefinesFile);

        var optPropertiesTemplateFile = new Option<FileInfo>(["-t", "template"], "An optional template entity.properties file to scan.");
        Add(optPropertiesTemplateFile);

        this.SetHandler(RunAsync, argPropertiesFile, argDefinesFile, optPropertiesTemplateFile);
    }

    private async Task RunAsync(FileInfo propertiesFile, FileInfo defineFile, FileInfo? templatePropertiesFile)
    {
        ArgumentNullException.ThrowIfNull(provider);
        var logger = provider.GetRequiredService<ILogger<EntityCommand>>();
        var processor = provider.GetRequiredService<EntityPropertiesProcessor>();

        logger.LogDebug("Generating entity-defines from file [{FullName}]...", propertiesFile.FullName);

        try {
            await processor.ProcessAsync(propertiesFile.FullName, defineFile.FullName, templatePropertiesFile?.FullName);

            logger.LogInformation("Successfully generated entity-defines for file [{FullName}]...", propertiesFile.FullName);
        }
        catch (Exception error) {
            logger.LogError(error, "Failed to generate entity-defines for file [{FullName}]!", propertiesFile.FullName);
        }
    }
}
