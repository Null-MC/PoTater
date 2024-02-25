using NullMC.APM.Internal.Parsers;

namespace NullMC.APM.Tests.Mock;

internal class MockPropertiesParser : IPropertiesParser
{
    private readonly Dictionary<string, string[]> groupList = new(StringComparer.InvariantCultureIgnoreCase);
    public IReadOnlyDictionary<string, string[]> Groups => groupList;


    public IAsyncEnumerable<ParsedLine> ParseAsync(TextReader reader, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
