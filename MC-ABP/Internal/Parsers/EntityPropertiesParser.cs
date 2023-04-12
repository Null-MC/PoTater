using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace MC_ABP.Internal.Parsers;

internal class EntityPropertiesParser : PropertiesParserBase
{
    private static readonly Regex expBlock = new(@"^entity\.([\d\*]+)\s*=\s*", RegexOptions.Singleline | RegexOptions.IgnoreCase);


    public EntityPropertiesParser(ILogger<EntityPropertiesParser> logger) : base(logger)
    {
        LineMatchExp = expBlock;
    }
}
