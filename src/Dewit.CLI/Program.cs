using System;
using System.IO;
using Dewit.Core.Interfaces;
using Dewit.Core.Services;
using Dewit.Data.Data;
using Dewit.Data.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Dewit.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure Logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/dewit.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Loading services and starting application.");

                // Configure service providers to enable DI
                var services = ConfigureServices();
                var serviceProvider = services.BuildServiceProvider();

                // Ensure db migrations are run
                var _db = serviceProvider.GetRequiredService<DewitDbContext>();
                _db.Database.Migrate();

                // Start the application
                serviceProvider.GetRequiredService<App>().Run(args);
            }
            catch (FileNotFoundException ex)
            {
                Log.Fatal(ex, "Unable to load config file. Exiting.");
                Console.WriteLine("ERROR : Unable to load config file. Please try again.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong during execution.");
                Console.WriteLine("ERROR : An error occured while executing the last task. Please try again.");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            // Make config available throughout the application
            var config = LoadConfiguration();
            services.AddSingleton(config);

            // Connect to database context
            services.AddDbContext<DewitDbContext>(opts => opts.UseSqlite(config.GetConnectionString("Sqlite")));

            // Register generic repository pattern
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            // Register service layer
            services.AddTransient<ITaskService, TaskService>();

            // Add structured logging
            services.AddLogging(builder =>
            {
                builder.AddSerilog(Log.Logger);
            });

            // Required to run the application
            services.AddTransient<App>();

            return services;
        }

        private static IConfiguration LoadConfiguration()
        {
            Log.Debug("Loading config file");
            try
            {
                var builder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("config.json");
                return builder.Build();
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"Error : {ex.Message}");
            }
        }
    }
}
