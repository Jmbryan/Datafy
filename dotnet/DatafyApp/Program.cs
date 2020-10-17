using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using ElectronCgi.DotNet;
using Datafy.Core;

namespace Datafy.App
{
    class Program
    {
        static Datafy.Core.ILogger Logger { get; set; }
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

            // Configure DB
            using var dbProvider = new SQLiteDbProvider<SQLiteDbContext>(new SQLiteDbContextFactory());
            dbProvider.Connect();

            // Conifgure manager
            var manager = new Manager();
            manager.DbProvider = dbProvider;
            Manager = manager;

            // Configure serializer
            var simpleFactory = new SimpleFactory();
            var simpleSerializer = new Datafy.App.Json.JsonSerializer(simpleFactory, writeIndented: true);

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
                driver.Serializer = simpleSerializer;
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
                    RootDirectory = Path.Combine(rootDirectory, "data")
                };
                config.TypeDirectory = Path.Combine(rootDirectory, Path.Combine(config.RootDirectory, "Types"));
                config.ObjectDirectory = Path.Combine(rootDirectory, Path.Combine(config.RootDirectory, "Objects"));

                if (!activeDriver.Initialize(config))
                {
                    Logger.WriteLine($"Failed to initialize driver {activeDriver.Name}");
                }
                else
                {
                    using var loadTransaction = new Transaction(manager);
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
            connection.On<string>("gettypenames", () =>
            {
                // TODO: consider MemoryCache
                var sb = new System.Text.StringBuilder();
                foreach (var type in Manager.TypeList)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(',');
                    }
                    sb.Append(type.Name);
                }
                return sb.ToString();
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

            foreach (System.Type type in assembly.GetTypes())
            {
                if (typeof(TPlugin).IsAssignableFrom(type))
                {
                    if (System.Activator.CreateInstance(type) is TPlugin result)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new System.ApplicationException(
                    $"Can't find any type which implements {typeof(TPlugin).Name} in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
