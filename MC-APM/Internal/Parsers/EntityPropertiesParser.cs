using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace NullMC.APM.Internal.Parsers;

internal partial class EntityPropertiesParser : PropertiesParserBase
{
    public EntityPropertiesParser(ILogger<EntityPropertiesParser> logger) : base(logger)
    {
        LineMatchExp = EntityLineRegex();
    }

    [GeneratedRegex(@"^entity\.([\d\*]+)\s*=\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex EntityLineRegex();
}
