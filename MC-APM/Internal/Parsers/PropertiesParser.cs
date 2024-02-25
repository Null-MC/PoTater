using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace NullMC.APM.Internal.Parsers;

internal interface IPropertiesParser
{
    IReadOnlyDictionary<string, string[]> Groups {get;}

    IAsyncEnumerable<ParsedLine> ParseAsync(TextReader reader, CancellationToken token = default);
}

internal abstract partial class PropertiesParserBase(ILogger<IPropertiesParser> logger) : IPropertiesParser
{
    private static readonly Regex expGroup = RegexGroup();
    private static readonly Regex expComment = RegexComment();
    private static readonly char[] whitespaceChars = [' ', '\t'];

    private readonly Dictionary<string, string[]> _groups = new(StringComparer.InvariantCultureIgnoreCase);

    protected Regex? LineMatchExp {get; set;}

    public IReadOnlyDictionary<string, string[]> Groups => _groups;


    public async IAsyncEnumerable<ParsedLine> ParseAsync(TextReader reader, [EnumeratorCancellation] CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(LineMatchExp);

        string? lastComment = null;
        var lineBuilder = new StringBuilder();

        while (await reader.ReadLineAsync(token) is { } line) {
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

            var groupMatch = expGroup.Match(lineFinal);
            if (groupMatch.Success) {
                var matchPos = lineFinal.IndexOf('=');
                var groupName = groupMatch.Groups[1].Value.Trim();

                _groups[groupName] = lineFinal[(matchPos + 1)..]
                    .Split(whitespaceChars, StringSplitOptions.RemoveEmptyEntries);

                lastComment = null;
                continue;
            }

            var commentMatch = expComment.Match(lineFinal);
            if (commentMatch.Success && !IsPreprocessor(lineFinal)) {
                lastComment = commentMatch.Groups[1].Value.Trim();
                continue;
            }

            var lineMatch = LineMatchExp.Match(lineFinal);
            if (!lineMatch.Success) {
                lastComment = null;

                yield return new ParsedLine {
                    Text = lineFinal,
                };

                continue;
            }

            var itemId = lineMatch.Groups[1].Value;

            if (lastComment != null) {
                var matchPos = lineFinal.IndexOf('=');
                var itemValue = lineFinal[(matchPos + 1)..].Trim();

                yield return new ParsedLine {
                    Id = itemId,
                    Name = lastComment,
                    BlockMatches = itemValue,
                };

                lastComment = null;
            }
            else {
                logger.LogWarning("No name comment found for line {blockId}.", itemId);
            }
        }
    }

    private static bool IsPreprocessor(string line)
    {
        return line.StartsWith("#if")
            || line.StartsWith("#elif")
            || line.StartsWith("#else")
            || line.StartsWith("#endif")
            || line.StartsWith("#include");
    }

    [GeneratedRegex(@"^#+\s*(\w+)\s*$", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex RegexComment();

    [GeneratedRegex(@"^group\.(\w+)\s*=\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex RegexGroup();
}
