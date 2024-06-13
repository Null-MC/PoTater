using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace NullMC.Potater.Internal.Parsers;

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
    private static readonly char[] nameSplitChars = [' ', '\t', ',', ';', '|'];

    private readonly Dictionary<string, string[]> groupList = new(StringComparer.InvariantCultureIgnoreCase);

    protected Regex? LineMatchExp {get; set;}

    public IReadOnlyDictionary<string, string[]> Groups => groupList;


    public async IAsyncEnumerable<ParsedLine> ParseAsync(TextReader reader, [EnumeratorCancellation] CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(LineMatchExp);

        string? lastComment = null;
        var lineBuilder = new StringBuilder();

        while (await reader.ReadLineAsync(token) is { } line) {
            token.ThrowIfCancellationRequested();

            if (line.TrimEnd().EndsWith('\\')) {
                lineBuilder.AppendLine(line.TrimEnd());
                continue;
            }

            lineBuilder.Append(line.TrimEnd());
            var lineFinal = lineBuilder.ToString();
            lineBuilder.Clear();

            var groupMatch = expGroup.Match(lineFinal);
            if (groupMatch.Success) {
                var groupMatchPos = lineFinal.IndexOf('=');
                var groupName = groupMatch.Groups[1].Value.Trim();

                groupList[groupName] = lineFinal[(groupMatchPos + 1)..]
                    .Split(whitespaceChars, StringSplitOptions.RemoveEmptyEntries);

                lastComment = null;
                continue;
            }

            var commentMatch = expComment.Match(lineFinal);
            if (commentMatch.Success) {
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

            var entryId = lineMatch.Groups[1].Value;

            var lineMatchPos = lineFinal.IndexOf('=');
            var itemValue = lineFinal[(lineMatchPos + 1)..].Trim();

            if (lastComment != null) {
                yield return new ParsedLine {
                    Id = entryId,
                    DefineNames = lastComment.Split(nameSplitChars, StringSplitOptions.RemoveEmptyEntries),
                    Matches = itemValue,
                };

                lastComment = null;
            }
            else {
                logger.LogWarning("No name comment found for line {entryId}.", entryId);

                yield return new ParsedLine {
                    Id = entryId,
                    //Names = lastComment.Split(nameSplitChars, StringSplitOptions.RemoveEmptyEntries),
                    Matches = itemValue,
                };
            }
        }
    }

    [GeneratedRegex(@"^\s*#=+\s*(.+)$", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex RegexComment();

    [GeneratedRegex(@"^\s*group\.(\w+)\s*=\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex RegexGroup();
}
