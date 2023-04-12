using System.CommandLine;

namespace NullMC.APM.Internal.Commands;

internal class AppRootCommand : RootCommand
{
    public Option<bool> VerboseOption {get;}


    public AppRootCommand(
        BlockCommand cmdBlock,
        ItemCommand cmdItem,
        EntityCommand cmdEntity)
    {
        Add(cmdBlock);
        Add(cmdItem);
        Add(cmdEntity);

        Add(VerboseOption = new Option<bool>(new[] {"--verbose", "-v"}, () => false, "Show the deets."));
    }
}
