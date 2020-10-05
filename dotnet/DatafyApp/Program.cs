using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using DatafyCore;
using ElectronCgi.DotNet;

namespace DatafyApp
{
    class Logger
    {
        public static void WriteLine(string message) => Console.Error.WriteLine(message);
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Load drivers
            string[] driverPaths = new string[]
            {
                @"DatafyDriverJSON\bin\Debug\netcoreapp3.1\DatafyDriverJSON.dll"
            };
            IEnumerable<IDriver> drivers = driverPaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreatePlugins<IDriver>(pluginAssembly);
            }).ToList();
            foreach (IDriver driver in drivers)
            {
                Logger.WriteLine($"{driver.Name}\t - {driver.Description}");
            }

            // Load exporters
            string[] exporterPaths = new string[]
            {
                @"DatafyExporterJSON\bin\Debug\netcoreapp3.1\DatafyExporterJSON.dll"
            };
            IEnumerable<IExporter> exporters = exporterPaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreatePlugins<IExporter>(pluginAssembly);
            }).ToList();
            foreach (IExporter exporter in exporters)
            {
                Logger.WriteLine($"{exporter.Name}\t - {exporter.Description}");
            }

            // Open connection to Electron
            var connection = new ConnectionBuilder()
                .WithLogging(minimumLogLevel: LogLevel.Trace, logFilePath: "logs/Datafy.txt")
                .Build();
            //connection.On<string, string>("greeting", name => "Hello " + name);
            connection.On<string, string>("greeting", name =>
            {
                //Logger.WriteLine($"On greeting: {name}");
                return "Hello " + name;
            });
            //connection.OnAsync<string, string>("greeting", async name => "Hello " + name);

            connection.Listen();
        }

        static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Logger.WriteLine($"Loading plugins from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        static IEnumerable<TPlugin> CreatePlugins<TPlugin>(Assembly assembly) where TPlugin : IPlugin
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(TPlugin).IsAssignableFrom(type))
                {
                    if (Activator.CreateInstance(type) is TPlugin result)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements {typeof(TPlugin).Name} in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
