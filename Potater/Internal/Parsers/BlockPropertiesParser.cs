using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace NullMC.Potater.Internal.Parsers;

internal partial class BlockPropertiesParser : PropertiesParserBase
{
    public BlockPropertiesParser(ILogger<BlockPropertiesParser> logger) : base(logger)
    {
        LineMatchExp = BlockLineRegex();
    }

    [GeneratedRegex(@"^\s*block\.([\d\*]+)\s*=\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex BlockLineRegex();
}
