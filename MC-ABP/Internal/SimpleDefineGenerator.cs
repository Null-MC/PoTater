using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MC_ABP.Internal;

internal interface ISimpleDefineGenerator
{
    Task GenerateAsync(string blockPropertiesFile, string blockDefineFile, string? blockPropertiesTemplateFile = null, CancellationToken token = default);
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

    public async Task GenerateAsync(string blockPropertiesFile, string blockDefineFile, string? blockPropertiesTemplateFile = null, CancellationToken token = default)
    {
        var inputFile = blockPropertiesTemplateFile ?? blockPropertiesFile;
        await using var inputStream = File.Open(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(inputStream);

        await using var defineOutputStream = File.Open(blockDefineFile, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var defineWriter = new StreamWriter(defineOutputStream);

        Stream? blockOutputStream = null;
        StreamWriter? blockWriter = null;
        try {
            if (blockPropertiesTemplateFile != null) {
                blockOutputStream = File.Open(blockPropertiesFile, FileMode.Create, FileAccess.Write, FileShare.None);
                blockWriter = new StreamWriter(blockOutputStream);
            }

            int? lastKnownIndex = null;
            await foreach (var (blockId, blockName, blockMatches) in ProcessAsync(reader, token)) {
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

                await defineWriter.WriteLineAsync($"#define {blockName} {blockIdFinal}");
                logger.LogDebug("Added {blockName}={blockIdFinal}", blockName, blockIdFinal);

                if (blockWriter != null) {
                    await blockWriter.WriteLineAsync($"block.{blockIdFinal}={blockMatches}");
                }
            }
        }
        finally {
            if (blockWriter != null)
                await blockWriter.DisposeAsync();

            if (blockOutputStream != null)
                await blockOutputStream.DisposeAsync();
        }
    }

    private async IAsyncEnumerable<(string id, string name, string blockMatches)> ProcessAsync(TextReader reader, [EnumeratorCancellation] CancellationToken token)
    {
        string? lastComment = null;
        var lineBuilder = new StringBuilder();

        while (await reader.ReadLineAsync() is {} line) {
            token.ThrowIfCancellationRequested();

            if (lineBuilder.Length > 0) lineBuilder.Append(' ');

            var lineTrimmed = line.Trim();
            if (lineTrimmed.EndsWith('\\')) {
                lineBuilder.Append(lineTrimmed[..^1].TrimEnd());
                continue;
            }

            lineBuilder.Append(lineTrimmed.TrimEnd());
            var lineFinal = lineBuilder.ToString();
            lineBuilder.Clear();

            var commentMatch = expComment.Match(lineFinal);
            if (commentMatch.Success) {
                lastComment = commentMatch.Groups[1].Value.Trim();
                continue;
            }

            var blockMatch = expBlock.Match(lineFinal);
            if (!blockMatch.Success) {
                lastComment = null;
                continue;
            }
            
            var blockId = blockMatch.Groups[1].Value;

            if (lastComment != null) {
                var blockMatchPos = lineFinal.IndexOf('=');
                var blockMatches = lineFinal[(blockMatchPos+1)..].Trim();

                yield return (blockId, lastComment, blockMatches);
                lastComment = null;
            }
            else {
                logger.LogWarning("No name comment found for block {blockId}.", blockId);
            }
        }
    }
}
