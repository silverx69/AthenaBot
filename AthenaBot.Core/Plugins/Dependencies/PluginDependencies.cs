namespace AthenaBot.Plugins.Dependencies
{
    public class PluginDependencies
    {
        public RuntimeTarget RuntimeTarget { get; set; }

        public Dictionary<string, Dictionary<string, Target>> Targets { get; set; }

        public Dictionary<string, Library> Libraries { get; set; }

        public PluginDependencies() {
            RuntimeTarget = new RuntimeTarget();
            Targets = new Dictionary<string, Dictionary<string, Target>>();
            Libraries = new Dictionary<string, Library>();
        }
    }
}
