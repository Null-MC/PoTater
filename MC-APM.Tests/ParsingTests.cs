using Microsoft.Extensions.DependencyInjection;
using NullMC.APM.Internal.Parsers;
using NullMC.APM.Tests.Internal;
using System.Text;
using Xunit.Abstractions;

namespace NullMC.APM.Tests;

public class ParsingTests(ITestOutputHelper outputHelper) : TestBase(outputHelper)
{
    [Fact]
    public async Task CanParseProperties()
    {
        var blockProperties = new StringBuilder()
            .AppendLine("#BLOCK_TORCH")
            .AppendLine("block.8=torch");

        using var reader = new StringReader(blockProperties.ToString());
        var parser = Provider.GetRequiredService<BlockPropertiesParser>();
        var parseResults = await parser.ParseAsync(reader).ToArrayAsync();

        var result = Assert.Single(parseResults);
        Assert.Equal("8", result.Id);
        Assert.Equal("BLOCK_TORCH", result.Name);
        Assert.Equal("torch", result.BlockMatches);
    }

    [Fact]
    public async Task CanParsePropertyGroups()
    {
        var blockProperties = new StringBuilder()
            .AppendLine("group.candles=red_candle green_candle blue_candle");

        using var reader = new StringReader(blockProperties.ToString());
        var parser = Provider.GetRequiredService<BlockPropertiesParser>();
        await parser.ParseAsync(reader).ToArrayAsync();

        var result = Assert.Single(parser.Groups);
        Assert.Equal("candles", result.Key);
        Assert.Collection(result.Value,
            value => Assert.Equal("red_candle", value),
            value => Assert.Equal("green_candle", value),
            value => Assert.Equal("blue_candle", value));
    }
}
