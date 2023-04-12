using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MC_ABP.Internal.Parsers;

internal interface IPropertiesParser
{
    IAsyncEnumerable<(string id, string name, string blockMatches)> ParseAsync(TextReader reader, CancellationToken token = default);
}

internal abstract class PropertiesParserBase : IPropertiesParser
{
    private static readonly Regex expComment = new(@"^#+\s*(\w+)\s*$", RegexOptions.Singleline | RegexOptions.IgnoreCase);

    private readonly ILogger<IPropertiesParser> logger;

    protected Regex? LineMatchExp {get; set;}


    protected PropertiesParserBase(ILogger<IPropertiesParser> logger)
    {
        this.logger = logger;
    }

    public async IAsyncEnumerable<(string id, string name, string blockMatches)> ParseAsync(TextReader reader, [EnumeratorCancellation] CancellationToken token = default)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        if (LineMatchExp == null) throw new ArgumentNullException(nameof(LineMatchExp));

        string? lastComment = null;
        var lineBuilder = new StringBuilder();

        while (await reader.ReadLineAsync() is { } line) {
            token.ThrowIfCancellationRequested();

            if (lineBuilder.Length > 0) lineBuilder.Append(' ');

            var lineTrimmed = line.Trim();
            if (lineTrimmed.EndsWith('\\'))
            {
                lineBuilder.Append(lineTrimmed[..^1].TrimEnd());
                continue;
            }

            lineBuilder.Append(lineTrimmed.TrimEnd());
            var lineFinal = lineBuilder.ToString();
            lineBuilder.Clear();

            var commentMatch = expComment.Match(lineFinal);
            if (commentMatch.Success)
            {
                lastComment = commentMatch.Groups[1].Value.Trim();
                continue;
            }

            var lineMatch = LineMatchExp.Match(lineFinal);
            if (!lineMatch.Success)
            {
                lastComment = null;
                continue;
            }

            var itemId = lineMatch.Groups[1].Value;

            if (lastComment != null)
            {
                var matchPos = lineFinal.IndexOf('=');
                var itemValue = lineFinal[(matchPos + 1)..].Trim();

                yield return (itemId, lastComment, itemValue);
                lastComment = null;
            }
            else
            {
                logger.LogWarning("No name comment found for line {blockId}.", itemId);
            }
        }
    }
}
