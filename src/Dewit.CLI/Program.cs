using System;
using Dewit.Core.Interfaces;
using Dewit.Core.Services;
using Dewit.Core.Utils;
using Dewit.Data.Data;
using Dewit.Data.Data.Repositories;
using Microsoft.EntityFrameworkCore;
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

                // Seed mood descriptors into dedicated table
                var descriptorRepo = serviceProvider.GetRequiredService<IRepository<Dewit.Core.Entities.MoodDescriptorItem>>();
                MoodDescriptorDefaults.SeedIfMissing(descriptorRepo);

                // Seed default export filename config if missing
                var configService = serviceProvider.GetRequiredService<IConfigurationService>();
                if (!configService.KeyExists("export.csv.title"))
                    configService.SetValue("export.csv.title", "dewit_tasks");
                if (!configService.KeyExists("export.json.title"))
                    configService.SetValue("export.json.title", "dewit_tasks");

                // Start the application
                serviceProvider.GetRequiredService<App>().Run(args);
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

            // Connect to database context
            services.AddDbContext<DewitDbContext>(opts => opts.UseSqlite(DbConnectionString.Get()));

            // Register generic repository pattern
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            // Register service layer
            services.AddTransient<ITaskService, TaskService>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IDataConverter, DataConverterService>();
            services.AddTransient<IMoodService, MoodService>();

            // Add structured logging
            services.AddLogging(builder =>
            {
                builder.AddSerilog(Log.Logger);
            });

            // Required to run the application
            services.AddTransient<App>();

            return services;
        }
    }
}
