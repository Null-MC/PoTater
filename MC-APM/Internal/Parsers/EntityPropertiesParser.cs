using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace NullMC.APM.Internal.Parsers;

internal class EntityPropertiesParser : PropertiesParserBase
{
    private static readonly Regex expBlock = new(@"^entity\.([\d\*]+)\s*=\s*", RegexOptions.Singleline | RegexOptions.IgnoreCase);


    public EntityPropertiesParser(ILogger<EntityPropertiesParser> logger) : base(logger)
    {
        LineMatchExp = expBlock;
    }
}
