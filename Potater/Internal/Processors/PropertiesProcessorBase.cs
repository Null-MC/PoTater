using System.Text;
using Microsoft.Extensions.Logging;
using NullMC.Potater.Internal.Parsers;

namespace NullMC.Potater.Internal.Processors;

internal abstract class PropertiesProcessorBase(
    ILogger<PropertiesProcessorBase> logger,
    IPropertiesParser parser)
{
    private static readonly char[] whitespaceChars = [' ', '\t'];

    protected IPropertiesParser Parser => parser;
    protected string? PropertyPrefix {get; set;}


    public async Task ProcessAsync(string propertiesFile, string defineFile, string? templateFile = null, CancellationToken token = default)
    {
        var inputFile = templateFile ?? propertiesFile;
        await using var inputStream = File.Open(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read);

        await using var defineOutputStream = File.Open(defineFile, FileMode.Create, FileAccess.Write, FileShare.None);

        Stream? propertiesOutputStream = null;
        try {
            if (templateFile != null) {
                propertiesOutputStream = File.Open(propertiesFile, FileMode.Create, FileAccess.Write, FileShare.None);
            }

            using var reader = new StreamReader(inputStream, leaveOpen: true);
            await using var defineWriter = new StreamWriter(defineOutputStream, leaveOpen: true);

            StreamWriter? propertiesWriter = null;
            try {
                if (propertiesOutputStream != null)
                    propertiesWriter = new StreamWriter(propertiesOutputStream, leaveOpen: true);

                await ProcessInternalAsync(reader, defineWriter, propertiesWriter, token);
            }
            finally {
                if (propertiesWriter != null)
                    await propertiesWriter.DisposeAsync();
            }
        }
        finally {
            if (propertiesOutputStream != null)
                await propertiesOutputStream.DisposeAsync();
        }
    }

    protected virtual IEnumerable<string> OnFilterValue(string value)
    {
        var i = value.IndexOf(':');
        var name = i >= 0 ? value[..i] : value;
        var state = i >= 0 ? value[i..] : string.Empty;

        var groupStart = name.IndexOf('[');
        if (groupStart >= 0) {
            var groupEnd = name.IndexOf(']', groupStart);

            if (groupEnd >= 0) {
                var prefix = name[..groupStart];
                var suffix = name[(groupEnd+1)..];
                name = name[(groupStart+1)..groupEnd];

                if (Parser.Groups.TryGetValue(name, out var groupValues)) {
                    foreach (var groupValue in groupValues) {
                        foreach (var subValue in OnFilterValue(groupValue))
                            yield return $"{prefix}{subValue}{suffix}{state}";
                    }

                    yield break;
                }
            }
        }

        //if (name.StartsWith('[') && name.EndsWith(']')) {
        //    name = name[1..^1];
        //    if (Parser.Groups.TryGetValue(name, out var groupValues)) {
        //        foreach (var groupValue in groupValues) {
        //            foreach (var subValue in OnFilterValue($"{groupValue}{state}"))
        //                yield return subValue;
        //        }

        //        yield break;
        //    }
        //}

        yield return value;
    }

    internal async Task ProcessInternalAsync(TextReader reader, TextWriter defineWriter, TextWriter? propertiesWriter, CancellationToken token)
    {
        int? lastKnownIndex = null;
        await foreach (var lineData in parser.ParseAsync(reader, token)) {
            if (lineData.Text != null) {
                if (propertiesWriter != null)
                    await propertiesWriter.WriteLineAsync(lineData.Text);

                continue;
            }

            int lineIdFinal;

            if (lineData.Id == "*") {
                lastKnownIndex = (lastKnownIndex ?? 0) + 1;
                lineIdFinal = lastKnownIndex.Value;
            }
            else {
                if (int.TryParse(lineData.Id, out var lineIdValue)) {
                    if (lineIdValue <= lastKnownIndex)
                        throw new ApplicationException($"Hard-coded property index '{lineIdValue:N0}' is less than previous index '{lastKnownIndex.Value:N0}'!");

                    lastKnownIndex = lineIdValue;
                    lineIdFinal = lineIdValue;
                }
                else throw new ApplicationException($"Failed to parse ID '{lineData.Id}'!");
            }

            if (propertiesWriter != null)
                await AppendPropertyLineAsync(propertiesWriter, lineIdFinal, lineData.Matches);

            if (lineData.DefineNames != null) {
                foreach (var name in lineData.DefineNames) {
                    await defineWriter.WriteLineAsync($"#define {name} {lineIdFinal}");

                    logger.LogDebug("Defined {name}={lineIdFinal}", name, lineIdFinal);
                }
            }
            else {
                logger.LogWarning("No names found for entry!");
            }
        }
    }

    private async Task AppendPropertyLineAsync(TextWriter propertiesWriter, int lineIdFinal, string? lineValue)
    {
        if (lineValue != null) {
            var valueBuilder = new StringBuilder();

            foreach (var part in lineValue.Split(whitespaceChars, StringSplitOptions.RemoveEmptyEntries).SelectMany(OnFilterValue)) {
                if (valueBuilder.Length > 0) valueBuilder.Append(' ');
                valueBuilder.Append(part);
            }

            lineValue = valueBuilder.ToString();
        }

        await propertiesWriter.WriteLineAsync($"{PropertyPrefix}.{lineIdFinal}={lineValue}");
    }
}
