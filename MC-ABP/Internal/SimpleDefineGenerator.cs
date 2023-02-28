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
    private static readonly Regex expBlock = new(@"^block\.(\d+)\s*=\s*", RegexOptions.Singleline | RegexOptions.IgnoreCase);

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

        await foreach (var (blockId, blockName) in ProcessAsync(reader, token)) {
            await writer.WriteLineAsync($"#define {blockName} {blockId}");
            logger.LogDebug("Added {blockName}={blockId}", blockName, blockId);
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
