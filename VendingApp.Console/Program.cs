using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Extensions.Logging;
using NLog.Layouts;
using VendingApp.Console.Logging;
using VendingApp.Infrastructure.Data;
using VendingApp.Infrastructure.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace VendingApp.Console
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static ILogger Logger { get; set; }

        public static IConsole Console { get; private set; }
        public static App MainApp { get; set; }


        static void Main(string[] args)
        {
            System.Console.WriteLine("Welcome to Acme AE-VM2K18. Starting...");

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();

            var services = new ServiceCollection()
                .AddLogging()
                .AddDbContext<VendingDbContext>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]);

                    // to lower EF logging level
                    options.UseLoggerFactory(new LoggerFactoryProxy(new LoggerFactory()));
                })
                .AddScoped<IConfigService, ConfigService>()
                .AddScoped<IInventoryService, InventoryService>()
                .AddScoped<IDatabaseInitializer, DatabaseInitializer>()
                .AddScoped<App>()
                .AddSingleton<IConsole>(PhysicalConsole.Singleton)
                .BuildServiceProvider();
            
            ConfigureLogging(services);
            Console = services.GetRequiredService<IConsole>();
            try
            {
                var dbStarter = services.GetRequiredService<IDatabaseInitializer>();
                
                var configFolder = Path.Combine(AppContext.BaseDirectory, "Resources");
                Logger.LogInformation("Config folder: " + configFolder);
                dbStarter.Run(configFolder).Wait();

                MainApp = services.GetService<App>();
                MainApp.ShowHelp();
                MainApp.Run();
            }
            catch (ValidationException ex)
            {
                Logger.LogInformation(ex.Message);
                MainApp.Run();
            }
            catch (Exception ex)
            {
                Logger.LogCritical("Main app exception:", ex);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nException: " + ex.Message);
                Console.WriteLine("\n\nPlease check set proper SQL instance in appsettings.json#ConnectionStrings:DefaultConnection");
                Console.WriteLine("\nPress any key to quit");
                Console.ForegroundColor = ConsoleColor.White;
                System.Console.ReadKey(true);
            }
        }


        private static void ConfigureLogging(ServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("nlog.config");
            Logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        }

    }
}
