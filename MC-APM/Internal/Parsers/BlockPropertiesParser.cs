using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace NullMC.APM.Internal.Parsers;

internal partial class BlockPropertiesParser : PropertiesParserBase
{
    public BlockPropertiesParser(ILogger<BlockPropertiesParser> logger) : base(logger)
    {
        LineMatchExp = BlockLineRegex();
    }

    [GeneratedRegex(@"^block\.([\d\*]+)\s*=\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex BlockLineRegex();
}
