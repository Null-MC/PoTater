using Microsoft.Extensions.Logging;
using NullMC.Potater.Internal.Parsers;

namespace NullMC.Potater.Internal.Processors;

internal class ItemPropertiesProcessor : PropertiesProcessorBase
{
    public ItemPropertiesProcessor(
        ILogger<ItemPropertiesProcessor> logger,
        ItemPropertiesParser parser) : base(logger, parser)
    {
        PropertyPrefix = "item";
    }
}
