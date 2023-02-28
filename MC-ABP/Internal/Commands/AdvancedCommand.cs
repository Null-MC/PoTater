using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace MC_ABP.Internal.Commands;

internal class AdvancedCommand : Command
{
    private readonly ILogger<AdvancedCommand> logger;


    public AdvancedCommand(
        ILogger<AdvancedCommand> logger)
            : base("advanced", "Generates a blocks.glsl file defining block ID's using comments from block.properties.")
    {
        this.logger = logger;

        var argJsonBlocksFile = new Argument<FileInfo>("json", "The blocks.json file to scan.");
        Add(argJsonBlocksFile);

        var argPropertiesFile = new Argument<FileInfo>("properties", "The block.properties file to generate.");
        Add(argPropertiesFile);

        var argDefinesFile = new Argument<FileInfo>("defines", "The blocks.glsl file to generate.");
        Add(argDefinesFile);

        this.SetHandler(RunAsync, argJsonBlocksFile, argPropertiesFile, argDefinesFile);
    }

    private Task RunAsync(FileInfo jsonBlocksFile, FileInfo propertiesFile, FileInfo defineFile)
    {
        throw new NotImplementedException("Advanced generation has not been implemented yet!");
    }
}
