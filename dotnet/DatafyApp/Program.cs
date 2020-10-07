using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using ElectronCgi.DotNet;
using DatafyCore;

namespace DatafyApp
{
    class Program
    {
        static DatafyCore.ILogger Logger { get; set; }
        static IManager Manager { get; set; }

        static void Main(string[] args)
        {
            // Configure logger as early as possible
            string rootDirectory = GetRootDirectory();
            string logRelativeFilePath = "logs\\Datafy.txt";
            string logFilePath = Path.Combine(rootDirectory, logRelativeFilePath);
            var logger = new Logger(logFilePath);
            logger.Connect();
            Logger = logger;

            // Conifgure manager
            var manager = new Manager();
            Manager = manager;

            // Configure serializer
            var serializer = new DatafyCore.Json.JsonSerializer(writeIndented: true);

            // Configure drivers
            string[] driverPaths = new string[]
            {
                @"DatafyDriverFile\bin\Debug\netcoreapp3.1\DatafyDriverFile.dll"
            };
            IEnumerable<IDriver> drivers = driverPaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreatePlugins<IDriver>(pluginAssembly);
            }).ToList();
            IDriver activeDriver = null;
            foreach (IDriver driver in drivers)
            {
                driver.Logger = Logger;
                driver.Serializer = serializer;
                Logger.WriteLine($"{driver.Name}\t - {driver.Description}");
                if (driver.Name == "File Driver")
                {
                    activeDriver = driver;
                }
            }

            // Configure exporters
            string[] exporterPaths = new string[]
            {
                @"DatafyExporterFile\bin\Debug\netcoreapp3.1\DatafyExporterFile.dll"
            };
            IEnumerable<IExporter> exporters = exporterPaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreatePlugins<IExporter>(pluginAssembly);
            }).ToList();
            IExporter activeExporter = null;
            foreach (IExporter exporter in exporters)
            {
                exporter.Logger = Logger;
                Logger.WriteLine($"{exporter.Name}\t - {exporter.Description}");
                if (exporter.Name == "File Exporter")
                {
                    activeExporter = exporter;
                }
            }

            // Load all data
            if (activeDriver != null)
            {
                var config = new DriverConfig
                {
                    RootDirectory = Path.Combine(rootDirectory, "data"),
                    ClassDirectory = Path.Combine(rootDirectory, "data\\Classes")
                };
                if (!activeDriver.Initialize(config))
                {
                    Logger.WriteLine($"Failed to initialize driver {activeDriver.Name}");
                }
                else
                {
                    var loadTransaction = new Transaction(manager);
                    activeDriver.LoadAll(loadTransaction);
                    loadTransaction.Commit();
                }
            }

            // Open connection to Electron
            var connection = new ConnectionBuilder()
                .WithLogging(minimumLogLevel: LogLevel.Trace, logFilePath: logRelativeFilePath)
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

        static string GetRootDirectory()
        {
            return Path.GetDirectoryName(GetSolutionRootPath());
        }

        static string GetSolutionRootPath()
        {
            return Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));
        }

        static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = GetSolutionRootPath();

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
