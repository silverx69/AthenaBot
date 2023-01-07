using System.ComponentModel;

namespace AthenaBot.Plugins
{
    public interface IPluginContext<TPlugin> where TPlugin : IPlugin
    {
        string Name { get; }
        TPlugin Plugin { get; }
    }
}
