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
}
