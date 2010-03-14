using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using Ondit.Client;

namespace OnditBot {
    class Program {
        private ICollection<IPlugin> plugins = new HashSet<IPlugin>();
        private Client client;

        static void Main(string[] args) {
            (new Program()).Run();
        }

        void Run() {
            using(client = new Client("irc.slagg.org", 6667)) {
                Console.WriteLine("Connecting...");

                client.Connect();
                client.WaitForConnected();

                Console.WriteLine("Connected!");
                
                LoadPlugins();

                client.JoinChannel("#test");

                while(client.ConnectionStatus == ConnectionStatus.Connected) {
                    client.HandleMessageBlock();
                }
            }
        }

        static IEnumerable<Type> FindPluginTypes() {
            var assemblies = new List<Assembly>();
            assemblies.Add(Assembly.GetExecutingAssembly());

            var plugins = new List<Type>();

            foreach(var assembly in assemblies) {
                plugins.AddRange(GetAssemblyPlugins(assembly));
            }

            return plugins;
        }

        static IEnumerable<Type> GetAssemblyPlugins(Assembly assembly) {
            return assembly.GetTypes().Where((type) => !type.IsAbstract && type.GetInterface(@"IPlugin") != null);
        }

        void LoadPlugins() {
            foreach(var pluginType in FindPluginTypes()) {
                LoadPlugin(pluginType);
            }
        }

        void LoadPlugin(Type pluginType) {
            var ctor = pluginType.GetConstructor(new Type[] {});

            if(ctor == null) {
                throw new MissingMethodException(pluginType.Name, "Could not get constructor for plugin type");
            }

            var plugin = ctor.Invoke(new object[] { }) as IPlugin;

            if(plugin == null) {
                throw new InvalidDataException("Could not create instance of plugin type");
            }

            LoadPlugin(plugin);
        }

        void LoadPlugin(IPlugin plugin) {
            plugin.Client = client;

            plugins.Add(plugin);

            Console.WriteLine(string.Format("Loaded plugin: {0}", plugin.Name));
        }
    }
}
