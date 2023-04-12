using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace MC_ABP.Internal.Parsers;

internal class ItemPropertiesParser : PropertiesParserBase
{
    private static readonly Regex expItem = new(@"^item\.([\d\*]+)\s*=\s*", RegexOptions.Singleline | RegexOptions.IgnoreCase);


    public ItemPropertiesParser(ILogger<ItemPropertiesParser> logger) : base(logger)
    {
        LineMatchExp = expItem;
    }
}
