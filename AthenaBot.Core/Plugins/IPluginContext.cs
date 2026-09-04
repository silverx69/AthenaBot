namespace AthenaBot.Plugins
{
    public interface IPluginContext<TPlugin> where TPlugin : IPlugin
    {
        /// <summary>
        /// Gets the name of the plugin loaded into this context.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the instance of the plugin loaded into this context.
        /// </summary>
        TPlugin Plugin { get; }
    }
}
