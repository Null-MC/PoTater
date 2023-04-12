using Microsoft.Extensions.Logging;
using NullMC.APM.Internal.Parsers;

namespace NullMC.APM.Internal.Processors;

internal class ItemPropertiesProcessor : PropertiesProcessorBase
{
    public ItemPropertiesProcessor(
        ILogger<ItemPropertiesProcessor> logger,
        ItemPropertiesParser parser) : base(logger, parser)
    {
        PropertyPrefix = "item";
    }
}
