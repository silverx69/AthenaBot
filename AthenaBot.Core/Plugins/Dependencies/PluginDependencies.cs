namespace AthenaBot.Plugins.Dependencies
{
    public class PluginDependencies
    {
        public RuntimeTarget RuntimeTarget { get; set; } = new();

        public Dictionary<string, Dictionary<string, Target>> Targets { get; set; } = [];

        public Dictionary<string, Library> Libraries { get; set; } = [];
    }
}
