using Microsoft.Extensions.DependencyInjection;
using NullMC.Potater.Internal.Processors;
using NullMC.Potater.Tests.Internal;
using System.Text;
using Xunit.Abstractions;

namespace NullMC.Potater.Tests;

public class ProcessingTests : TestBase
{
    private static readonly char[] newlineChars = ['\r', '\n'];

    private readonly BlockPropertiesProcessor processor;


    public ProcessingTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        processor = Provider.GetRequiredService<BlockPropertiesProcessor>();
    }

    [Fact]
    public async Task CanAutoNumberProperties()
    {
        var blockPropertiesTemplate = new StringBuilder()
            .AppendLine("#=BLOCK_TORCH")
            .AppendLine("block.8=torch")
            .AppendLine("#=BLOCK_LANTERN")
            .AppendLine("block.*=lantern")
            .AppendLine("#=BLOCK_FIRE")
            .AppendLine("block.20=fire")
            .AppendLine("#=BLOCK_SOUL_FIRE")
            .AppendLine("block.*=soul_fire");

        await using var defineWriter = new StringWriter();
        await using var propertiesWriter = new StringWriter();
        using var propertiesReader = new StringReader(blockPropertiesTemplate.ToString());
        await processor.ProcessInternalAsync(propertiesReader, defineWriter, propertiesWriter, CancellationToken.None);

        Assert.Collection(defineWriter.ToString().Split(newlineChars, StringSplitOptions.RemoveEmptyEntries),
            line => Assert.Equal("#define BLOCK_TORCH 8", line),
            line => Assert.Equal("#define BLOCK_LANTERN 9", line),
            line => Assert.Equal("#define BLOCK_FIRE 20", line),
            line => Assert.Equal("#define BLOCK_SOUL_FIRE 21", line));
    }

    [Fact]
    public async Task CanApplyPropertyGroups()
    {
        var blockPropertiesTemplate = new StringBuilder()
            .AppendLine("group.candles=red_candle green_candle blue_candle")
            .AppendLine("#=BLOCK_CANDLES_1")
            .AppendLine("block.11=[candles]:candles=1:lit=false")
            .AppendLine("#=BLOCK_CANDLES_2")
            .AppendLine("block.12=[candles]:candles=2:lit=false")
            .AppendLine("#=BLOCK_CANDLES_3")
            .AppendLine("block.13=[candles]:candles=3:lit=false");

        await using var defineWriter = new StringWriter();
        await using var propertiesWriter = new StringWriter();
        using var propertiesReader = new StringReader(blockPropertiesTemplate.ToString());
        await processor.ProcessInternalAsync(propertiesReader, defineWriter, propertiesWriter, CancellationToken.None);

        Assert.Collection(defineWriter.ToString().Split(newlineChars, StringSplitOptions.RemoveEmptyEntries),
            line => Assert.Equal("#define BLOCK_CANDLES_1 11", line),
            line => Assert.Equal("#define BLOCK_CANDLES_2 12", line),
            line => Assert.Equal("#define BLOCK_CANDLES_3 13", line));

        Assert.Collection(propertiesWriter.ToString().Split(newlineChars, StringSplitOptions.RemoveEmptyEntries),
            line => Assert.Equal("block.11=red_candle:candles=1:lit=false green_candle:candles=1:lit=false blue_candle:candles=1:lit=false", line),
            line => Assert.Equal("block.12=red_candle:candles=2:lit=false green_candle:candles=2:lit=false blue_candle:candles=2:lit=false", line),
            line => Assert.Equal("block.13=red_candle:candles=3:lit=false green_candle:candles=3:lit=false blue_candle:candles=3:lit=false", line));
    }
}
