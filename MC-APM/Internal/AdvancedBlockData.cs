namespace NullMC.APM.Internal
{
    internal class AdvancedBlockData
    {
        public int? Id {get; set;}
        public string? Name {get; set;}
        public string[]? Matches {get; set;}
        public Dictionary<string, object>? Metadata {get; set;}
    }
}
