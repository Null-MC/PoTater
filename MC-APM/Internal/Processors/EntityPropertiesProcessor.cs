using MC_ABP.Internal.Parsers;
using Microsoft.Extensions.Logging;

namespace MC_ABP.Internal.Processors;

internal class EntityPropertiesProcessor : PropertiesProcessorBase
{
    public EntityPropertiesProcessor(
        ILogger<EntityPropertiesProcessor> logger,
        EntityPropertiesParser parser) : base(logger, parser)
    {
        PropertyPrefix = "entity";
    }
}
