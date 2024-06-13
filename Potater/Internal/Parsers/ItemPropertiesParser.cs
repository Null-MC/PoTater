using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace NullMC.Potater.Internal.Parsers;

internal partial class ItemPropertiesParser : PropertiesParserBase
{
    public ItemPropertiesParser(ILogger<ItemPropertiesParser> logger) : base(logger)
    {
        LineMatchExp = ItemLineRegex();
    }

    [GeneratedRegex(@"^item\.([\d\*]+)\s*=\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex ItemLineRegex();
}
