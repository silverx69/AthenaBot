namespace AthenaBot.Plugins.Dependencies
{
    public class Target
    {
        public Dictionary<string, string> Dependencies { get; set; } = [];

        public Dictionary<string, Dictionary<string, string>> Runtime { get; set; } = [];
    }
}
