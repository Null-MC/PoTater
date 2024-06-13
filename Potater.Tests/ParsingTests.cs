using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NullMC.Potater.Internal.Parsers;
using NullMC.Potater.Tests.Internal;
using Xunit.Abstractions;

namespace NullMC.Potater.Tests;

public class ParsingTests(ITestOutputHelper outputHelper) : TestBase(outputHelper)
{
    [Fact]
    public async Task CanParseSingleProperties()
    {
        var blockProperties = new StringBuilder()
            .AppendLine("#= BLOCK_TORCH")
            .AppendLine("block.8=torch");

        using var reader = new StringReader(blockProperties.ToString());
        var parser = Provider.GetRequiredService<BlockPropertiesParser>();
        var parseResults = await parser.ParseAsync(reader).ToArrayAsync();

        var result = Assert.Single(parseResults);
        Assert.Equal("8", result.Id);
        Assert.NotNull(result.DefineNames);
        var name = Assert.Single(result.DefineNames);
        Assert.Equal("BLOCK_TORCH", name);
        Assert.Equal("torch", result.Matches);
    }

    [Fact]
    public async Task CanParseMultipleProperties()
    {
        var blockProperties = new StringBuilder()
            .AppendLine("#= BLOCK_A, BLOCK_B")
            .AppendLine("block.8=torch");

        using var reader = new StringReader(blockProperties.ToString());
        var parser = Provider.GetRequiredService<BlockPropertiesParser>();
        var parseResults = await parser.ParseAsync(reader).ToArrayAsync();

        var result = Assert.Single(parseResults);
        Assert.Equal("8", result.Id);
        Assert.NotNull(result.DefineNames);
        Assert.Collection(result.DefineNames,
            r => Assert.Equal("BLOCK_A", r),
            r => Assert.Equal("BLOCK_B", r));
        Assert.Equal("torch", result.Matches);
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
