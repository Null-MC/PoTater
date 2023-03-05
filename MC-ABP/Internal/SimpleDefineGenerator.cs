using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MC_ABP.Internal;

internal interface ISimpleDefineGenerator
{
    Task GenerateAsync(string blockPropertiesFile, string blockDefineFile, CancellationToken token = default);
}

internal class SimpleDefineGenerator : ISimpleDefineGenerator
{
    private static readonly Regex expComment = new(@"^#+\s*(\w+)\s*$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
    private static readonly Regex expBlock = new(@"^block\.([\d\*]+)\s*=\s*", RegexOptions.Singleline | RegexOptions.IgnoreCase);

    private readonly ILogger<SimpleDefineGenerator> logger;


    public SimpleDefineGenerator(ILogger<SimpleDefineGenerator> logger)
    {
        this.logger = logger;
    }

    public async Task GenerateAsync(string blockPropertiesFile, string blockDefineFile, CancellationToken token = default)
    {
        await using var inputStream = File.Open(blockPropertiesFile, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(inputStream);

        await using var outputStream = File.Open(blockDefineFile, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var writer = new StreamWriter(outputStream);

        int? lastKnownIndex = null;
        await foreach (var (blockId, blockName) in ProcessAsync(reader, token)) {
            int blockIdFinal;

            if (blockId == "*") {
                lastKnownIndex = (lastKnownIndex ?? 0) + 1;
                blockIdFinal = lastKnownIndex.Value;
            }
            else {
                if (int.TryParse(blockId, out var blockIdValue)) {
                    lastKnownIndex = blockIdValue;
                    blockIdFinal = blockIdValue;
                }
                else throw new ApplicationException($"Failed to parse block ID '{blockId}'!");
            }

            await writer.WriteLineAsync($"#define {blockName} {blockIdFinal}");
            logger.LogDebug("Added {blockName}={blockIdFinal}", blockName, blockIdFinal);
        }
    }

    private async IAsyncEnumerable<(string id, string name)> ProcessAsync(TextReader reader, [EnumeratorCancellation] CancellationToken token)
    {
        string? lastComment = null;

        while (await reader.ReadLineAsync() is {} line) {
            token.ThrowIfCancellationRequested();

            var commentMatch = expComment.Match(line);
            if (commentMatch.Success) {
                lastComment = commentMatch.Groups[1].Value.Trim();
                continue;
            }

            var blockMatch = expBlock.Match(line);
            if (!blockMatch.Success) {
                lastComment = null;
                continue;
            }
            
            var blockId = blockMatch.Groups[1].Value;

            if (lastComment != null) {
                yield return (blockId, lastComment);
                lastComment = null;
            }
            else {
                logger.LogWarning("No name comment found for block {blockId}.", blockId);
            }
        }
    }
}
