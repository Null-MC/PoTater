using System.CommandLine;

namespace NullMC.APM.Internal.Commands;

internal class AppRootCommand : RootCommand
{
    public static Option<bool> DebugOption {get;} = new(["--debug", "-d"], () => false, "Enable debug logging.");

    public static AppRootCommand Instance {get;} = new(BlockCommand.Instance, ItemCommand.Instance, EntityCommand.Instance);


    public AppRootCommand(
        BlockCommand cmdBlock,
        ItemCommand cmdItem,
        EntityCommand cmdEntity)
    {
        Add(cmdBlock);
        Add(cmdItem);
        Add(cmdEntity);

        AddGlobalOption(DebugOption);
    }
}
