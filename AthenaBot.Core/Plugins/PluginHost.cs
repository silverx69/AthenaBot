namespace AthenaBot.Plugins
{
    public abstract class PluginHost<TPlugin> :
        ModelReadOnlyList<PluginContext<TPlugin>>,
        IPluginHost<TPlugin>
        where TPlugin : IPlugin
    {
        volatile bool unloading = false;

        public string BaseDirectory {
            get;
            private set;
        }

        public PluginHost(string baseDirectory) {
            BaseDirectory = baseDirectory;
        }

        public virtual void Dispose() {
            KillAllPlugins();
            GC.SuppressFinalize(this);
        }

        public bool IsPluginLoaded(string name) {
            string lowname = name.ToLower();

            if (lowname.EndsWith(".dll")) {
                name = name[0..^4];
                lowname = lowname[0..^4];
            }

            lock (SyncRoot) {
                int index = this.FindIndex(s => s.Name.ToLower() == lowname);
                if (index > -1) return true;
            }

            return false;
        }

        public bool LoadPlugin(string name) {
            if (IsPluginLoaded(name))
                return true;
            try {
                LoadPluginInternal(name);
            }
            catch (Exception ex) {
                Logging.Error(string.Format("{0}.{1}", GetType().Name, nameof(LoadPlugin)), ex);
            }
            return false;
        }

        public async Task<bool> LoadPluginAsync(string name) {
            if (IsPluginLoaded(name))
                return true;
            try {
                return await Task.Run(() => LoadPluginInternal(name));
            }
            catch (Exception ex) {
                await Logging.ErrorAsync(string.Format("{0}.{1}", GetType().Name, nameof(LoadPlugin)), ex);
            }
            return false;
        }

        private bool LoadPluginInternal(string name) {
            var context = GetPluginContext<TPlugin>(name);
            if (context == null) return false;

            context.Assembly = context.LoadFromAssemblyPath(context.FilePath);

            Type impl = null;
            Type pluginType = typeof(TPlugin);

            foreach (var type in context.Assembly.GetExportedTypes()) {
                if (pluginType.IsAssignableFrom(type))
                    impl = type;
            }

            if (impl == null)
                throw new PluginLoadException("Assembly does not contain a valid IPlugin implementation.");

            context.Plugin = (TPlugin)Activator.CreateInstance(impl);

            lock (SyncRoot) InnerList.Add(context);

            OnPluginLoaded(context);

            return true;
        }

        public void KillPlugin(string name) {
            lock (SyncRoot) {
                string lowname = name.ToLower();

                int index = this.FindIndex(s => s.Name.ToLower() == lowname);
                if (index == -1) return;

                KillPlugin(this[index]);
            }
        }

        public Task KillPluginAsync(string name) {
            return Task.Run(() => KillPlugin(name));
        }

        protected void KillPlugin(PluginContext<TPlugin> context) {
            InnerList.Remove(context);
            OnPluginKilled(context);
            context.Unload();
        }

        protected void KillAllPlugins() {
            unloading = true;
            foreach (var context in this) {
                OnPluginKilled(context);
                context.Unload();
            }
            InnerList.Clear();
            unloading = false;
        }

        protected abstract void OnPluginLoaded(PluginContext<TPlugin> context);

        protected abstract void OnPluginKilled(PluginContext<TPlugin> context);


        protected void RaisePluginLoaded(PluginContext<TPlugin> context) {
            Loaded?.Invoke(context);
        }

        protected void RaisePluginKilled(PluginContext<TPlugin> context) {
            if (!unloading) Killed?.Invoke(context);
        }

        protected virtual PluginContext<T> GetPluginContext<T>(string dllname) where T : IPlugin {
            return new PluginContext<T>(dllname, Path.Combine(BaseDirectory, dllname, dllname + ".dll"));
        }

        public event PluginEventHandler<TPlugin> Loaded;
        public event PluginEventHandler<TPlugin> Killed;
    }
}