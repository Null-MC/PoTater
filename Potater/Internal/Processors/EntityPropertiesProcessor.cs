using Microsoft.Extensions.Logging;
using NullMC.Potater.Internal.Parsers;

namespace NullMC.Potater.Internal.Processors;

internal class EntityPropertiesProcessor : PropertiesProcessorBase
{
    public EntityPropertiesProcessor(
        ILogger<EntityPropertiesProcessor> logger,
        EntityPropertiesParser parser) : base(logger, parser)
    {
        PropertyPrefix = "entity";
    }
}
