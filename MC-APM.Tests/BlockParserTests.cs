using Microsoft.Extensions.DependencyInjection;
using NullMC.APM.Internal.Parsers;
using NullMC.APM.Tests.Internal;
using System.Text;
using Xunit.Abstractions;

namespace NullMC.APM.Tests;

public class BlockParserTests : TestBase
{
    public BlockParserTests(ITestOutputHelper outputHelper) : base(outputHelper) {}

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
        Assert.Equal("8", result.id);
        Assert.Equal("BLOCK_TORCH", result.name);
        Assert.Equal("torch", result.blockMatches);
    }
}
