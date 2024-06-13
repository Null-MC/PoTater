using Microsoft.Extensions.Logging;
using NullMC.Potater.Internal.Parsers;

namespace NullMC.Potater.Internal.Processors;

internal class BlockPropertiesProcessor : PropertiesProcessorBase
{
    public BlockPropertiesProcessor(
        ILogger<BlockPropertiesProcessor> logger,
        BlockPropertiesParser parser) : base(logger, parser)
    {
        PropertyPrefix = "block";
    }
}
