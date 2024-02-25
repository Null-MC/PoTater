using Microsoft.Extensions.Logging;
using NullMC.APM.Internal.Parsers;

namespace NullMC.APM.Internal.Processors;

internal class BlockPropertiesProcessor : PropertiesProcessorBase
{
    public BlockPropertiesProcessor(
        ILogger<BlockPropertiesProcessor> logger,
        BlockPropertiesParser parser) : base(logger, parser)
    {
        PropertyPrefix = "block";
    }

    protected override IEnumerable<string> OnFilterValue(string value)
    {
        var i = value.IndexOf(':');
        var name = i >= 0 ? value[..i] : value;
        var state = i >= 0 ? value[i..] : string.Empty;

        // TODO: expand groups
        if (name.StartsWith('[') && name.EndsWith(']')) {
            name = name[1..^1];
            if (Parser.Groups.TryGetValue(name, out var groupValues)) {
                foreach (var groupValue in groupValues)
                    yield return $"{groupValue}{state}";

                yield break;
            }
        }

        yield return value;
    }
}
