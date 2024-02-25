using Microsoft.Extensions.DependencyInjection;
using NullMC.APM.Internal.Parsers;
using NullMC.APM.Tests.Internal;
using System.Text;
using Xunit.Abstractions;

namespace NullMC.APM.Tests;

public class BlockParserTests(ITestOutputHelper outputHelper) : TestBase(outputHelper)
{
    [Fact]
    public async Task CanParseBlockProperties()
    {
        var blockProperties = new StringBuilder()
            .AppendLine("#BLOCK_TORCH")
            .AppendLine("block.8=torch");

        var parser = Provider.GetRequiredService<BlockPropertiesParser>();
        using var reader = new StringReader(blockProperties.ToString());
        var parseResults = await parser.ParseAsync(reader).ToArrayAsync();

        var result = Assert.Single(parseResults);
        Assert.Equal("8", result.Id);
        Assert.Equal("BLOCK_TORCH", result.Name);
        Assert.Equal("torch", result.BlockMatches);
    }
}
