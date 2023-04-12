using MC_ABP.Internal.Parsers;
using Microsoft.Extensions.Logging;

namespace MC_ABP.Internal.Processors;

internal class ItemPropertiesProcessor : PropertiesProcessorBase
{
    public ItemPropertiesProcessor(
        ILogger<ItemPropertiesProcessor> logger,
        ItemPropertiesParser parser) : base(logger, parser)
    {
        PropertyPrefix = "item";
    }
}
