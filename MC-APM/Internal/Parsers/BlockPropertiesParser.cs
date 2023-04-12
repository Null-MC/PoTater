using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace NullMC.APM.Internal.Parsers;

internal class BlockPropertiesParser : PropertiesParserBase
{
    private static readonly Regex expBlock = new(@"^block\.([\d\*]+)\s*=\s*", RegexOptions.Singleline | RegexOptions.IgnoreCase);


    public BlockPropertiesParser(ILogger<BlockPropertiesParser> logger) : base(logger)
    {
        LineMatchExp = expBlock;
    }
}
