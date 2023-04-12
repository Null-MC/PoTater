using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace NullMC.APM.Internal.Parsers;

internal class ItemPropertiesParser : PropertiesParserBase
{
    private static readonly Regex expItem = new(@"^item\.([\d\*]+)\s*=\s*", RegexOptions.Singleline | RegexOptions.IgnoreCase);


    public ItemPropertiesParser(ILogger<ItemPropertiesParser> logger) : base(logger)
    {
        LineMatchExp = expItem;
    }
}
