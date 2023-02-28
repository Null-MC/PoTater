using System.CommandLine;

namespace MC_ABP.Internal.Commands;

internal class AppRootCommand : RootCommand
{
    public Option<bool> VerboseOption {get;}


    public AppRootCommand(
        SimpleCommand cmdSimple,
        AdvancedCommand cmdAdvanced)
    {
        Add(cmdSimple);
        Add(cmdAdvanced);

        Add(VerboseOption = new Option<bool>(new[] {"--verbose", "-v"}, () => false, "Show the deets."));
    }
}
