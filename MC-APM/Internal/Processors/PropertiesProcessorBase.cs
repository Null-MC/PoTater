using Microsoft.Extensions.Logging;
using NullMC.APM.Internal.Parsers;

namespace NullMC.APM.Internal.Processors;

internal interface IPropertiesProcessor
{
    Task ProcessAsync(string propertiesFile, string defineFile, string? templateFile = null, CancellationToken token = default);
}

internal abstract class PropertiesProcessorBase : IPropertiesProcessor
{
    private readonly ILogger<PropertiesProcessorBase> logger;
    private readonly IPropertiesParser parser;

    protected string? PropertyPrefix {get; set;}


    protected PropertiesProcessorBase(
        ILogger<PropertiesProcessorBase> logger,
        IPropertiesParser parser)
    {
        this.parser = parser;
        this.logger = logger;
    }

    public async Task ProcessAsync(string propertiesFile, string defineFile, string? templateFile = null, CancellationToken token = default)
    {
        var inputFile = templateFile ?? propertiesFile;
        await using var inputStream = File.Open(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(inputStream);

        await using var defineOutputStream = File.Open(defineFile, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var defineWriter = new StreamWriter(defineOutputStream);

        Stream? propertiesOutputStream = null;
        StreamWriter? propertiesWriter = null;
        try {
            if (templateFile != null) {
                propertiesOutputStream = File.Open(propertiesFile, FileMode.Create, FileAccess.Write, FileShare.None);
                propertiesWriter = new StreamWriter(propertiesOutputStream);
            }

            int? lastKnownIndex = null;
            await foreach (var (itemId, itemName, itemMatches) in parser.ParseAsync(reader, token)) {
                int itemIdFinal;

                if (itemId == "*") {
                    lastKnownIndex = (lastKnownIndex ?? 0) + 1;
                    itemIdFinal = lastKnownIndex.Value;
                }
                else {
                    if (int.TryParse(itemId, out var blockIdValue)) {
                        if (blockIdValue <= lastKnownIndex)
                            throw new ApplicationException($"Hard-coded property index '{blockIdValue:N0}' is less than previous index '{lastKnownIndex.Value:N0}'!");

                        lastKnownIndex = blockIdValue;
                        itemIdFinal = blockIdValue;
                    }
                    else throw new ApplicationException($"Failed to parse ID '{itemId}'!");
                }

                await defineWriter.WriteLineAsync($"#define {itemName} {itemIdFinal}");
                logger.LogDebug("Added {itemName}={itemIdFinal}", itemName, itemIdFinal);

                if (propertiesWriter != null) {
                    await propertiesWriter.WriteLineAsync($"{PropertyPrefix}.{itemIdFinal}={itemMatches}");
                }
            }
        }
        finally {
            if (propertiesWriter != null)
                await propertiesWriter.DisposeAsync();

            if (propertiesOutputStream != null)
                await propertiesOutputStream.DisposeAsync();
        }
    }
}
