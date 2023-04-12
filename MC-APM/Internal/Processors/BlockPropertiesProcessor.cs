using MC_ABP.Internal.Parsers;
using Microsoft.Extensions.Logging;

namespace MC_ABP.Internal.Processors;

internal class BlockPropertiesProcessor : PropertiesProcessorBase
{
    public BlockPropertiesProcessor(
        ILogger<BlockPropertiesProcessor> logger,
        BlockPropertiesParser parser) : base(logger, parser)
    {
        PropertyPrefix = "block";
    }
}
