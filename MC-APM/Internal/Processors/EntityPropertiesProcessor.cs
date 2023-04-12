using Microsoft.Extensions.Logging;
using NullMC.APM.Internal.Parsers;

namespace NullMC.APM.Internal.Processors;

internal class EntityPropertiesProcessor : PropertiesProcessorBase
{
    public EntityPropertiesProcessor(
        ILogger<EntityPropertiesProcessor> logger,
        EntityPropertiesParser parser) : base(logger, parser)
    {
        PropertyPrefix = "entity";
    }
}
