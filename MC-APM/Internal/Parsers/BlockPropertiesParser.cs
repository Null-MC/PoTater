using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace MC_ABP.Internal.Parsers;

internal class BlockPropertiesParser : PropertiesParserBase
{
    private static readonly Regex expBlock = new(@"^block\.([\d\*]+)\s*=\s*", RegexOptions.Singleline | RegexOptions.IgnoreCase);


    public BlockPropertiesParser(ILogger<BlockPropertiesParser> logger) : base(logger)
    {
        LineMatchExp = expBlock;
    }
}
