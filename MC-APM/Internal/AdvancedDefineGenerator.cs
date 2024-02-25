using System.Text;
using System.Text.Json.Nodes;

namespace NullMC.APM.Internal;

internal interface IAdvancedDefineGenerator
{
    Task GenerateAsync(string blocksJsonFile, string blockPropertiesFile, string blockDefinesFile, CancellationToken token = default);
}

internal class AdvancedDefineGenerator : IAdvancedDefineGenerator
{
    public async Task GenerateAsync(string blocksJsonFile, string blockPropertiesFile, string blockDefinesFile, CancellationToken token = default)
    {
        var blockData = ParseBlockJsonData(blocksJsonFile).ToArray();

        var allMetadataKeys = blockData
            .SelectMany(d => d.Metadata?.Keys ?? Enumerable.Empty<string>())
            .Distinct().ToArray();

        var propertiesBuilder = new StringBuilder();
        //var definesBuilder = new StringBuilder();

        var allMetadata = new Dictionary<string, object?[]>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var key in allMetadataKeys) allMetadata[key] = new object[blockData.Length];

        //await using var blockPropertiesStream = File.Open(blockPropertiesFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        await using var blockDefinesStream = File.Open(blockDefinesFile, FileMode.Create, FileAccess.Write);
        await using var blockDefinesWriter = new StreamWriter(blockDefinesStream);

        var index = 0;
        foreach (var item in blockData) {
            token.ThrowIfCancellationRequested();

            if (item.Matches != null)
                propertiesBuilder.AppendLine($"block.{item.Id}={string.Join(' ', item.Matches)}");

            //definesBuilder.AppendLine($"#define {item.Name} {item.Id}");
            await blockDefinesWriter.WriteLineAsync($"#define {item.Name} {item.Id}");

            foreach (var key in allMetadataKeys)
                item.Metadata?.TryGetValue(key, out allMetadata[key][index]);

            index++;
        }

        if (allMetadataKeys.Length > 0) {
            await blockDefinesWriter.WriteLineAsync();

            foreach (var key in allMetadataKeys) {
                // TODO: need to predetermine data type!
                var dataType = "float";

                await blockDefinesWriter.WriteAsync($"{dataType} {key}[] = {{");

                var i = 0;
                foreach (var value in allMetadata[key]) {
                    if (i > 0) await blockDefinesWriter.WriteAsync(", ");

                    // TODO: write value formatted as data type
                    await blockDefinesWriter.WriteAsync("0.0");

                    i++;
                }

                await blockDefinesWriter.WriteLineAsync("};");
            }
        }

        // TODO: create/inject block properties
        var blockPropertiesText = propertiesBuilder.ToString();
    }

    public IEnumerable<AdvancedBlockData> ParseBlockJsonData(string blocksJsonFile)
    {
        using var blocksJsonStream = File.Open(blocksJsonFile, FileMode.Open, FileAccess.Read);

        var jsonData = JsonNode.Parse(blocksJsonStream)?.AsObject()
            ?? throw new ApplicationException("No data found!");

        var index = jsonData["__start"]?.GetValue<int>() ?? 1;

        foreach (var itemData in jsonData) {
            if (itemData.Key.StartsWith('_')) continue;

            var itemObj = itemData.Value?.AsObject();
            if (itemObj == null) continue;

            var item = new AdvancedBlockData {
                Id = index,
                Name = itemData.Key,
            };

            if (itemObj["__match"] is JsonArray matchArray) {
                item.Matches = matchArray.Select(n => n?.GetValue<string>() ?? string.Empty).ToArray();
            }
            else {
                var matchStr = itemObj["__match"]?.GetValue<string>();
                if (matchStr != null) {
                    item.Matches = matchStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                }
            }

            foreach (var itemProp in itemObj) {
                if (itemProp.Key.StartsWith('_') || itemProp.Value == null) continue;

                item.Metadata ??= new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
                item.Metadata[itemProp.Key] = itemProp.Value;
            }

            yield return item;
            index++;
        }
    }
}
