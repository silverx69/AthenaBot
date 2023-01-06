namespace AthenaBot.Plugins
{
    public sealed class PluginErrorInfo : IErrorInfo
    {
        /// <summary>
        /// The name of the plugin where the error happened.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The name of the method, or identifying label, where the error happened.
        /// </summary>
        public string Method { get; internal set; }

        /// <summary>
        /// The exception that occured in the plugin to cause the error.
        /// </summary>
        public Exception Exception { get; internal set; }

        public PluginErrorInfo(string name, string method, Exception ex) {
            Name = name;
            Method = method;
            Exception = ex;
        }
    }
}