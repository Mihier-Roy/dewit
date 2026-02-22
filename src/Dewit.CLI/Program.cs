using System;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Dewit.Core.Services;
using Dewit.Core.Utils;
using Dewit.Data.Data;
using Dewit.Data.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dewit.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var services = ConfigureServices();
                var serviceProvider = services.BuildServiceProvider();

                var _db = serviceProvider.GetRequiredService<DewitDbContext>();
                _db.Database.Migrate();

                var descriptorRepo = serviceProvider.GetRequiredService<
                    IRepository<Dewit.Core.Entities.MoodDescriptorItem>
                >();
                MoodDescriptorDefaults.SeedIfMissing(descriptorRepo);

                var configService = serviceProvider.GetRequiredService<IConfigurationService>();
                if (!configService.KeyExists("export.csv.title"))
                    configService.SetValue("export.csv.title", "dewit_tasks");
                if (!configService.KeyExists("export.json.title"))
                    configService.SetValue("export.json.title", "dewit_tasks");

                serviceProvider.GetRequiredService<App>().Run(args);
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Unhandled exception");
                Console.WriteLine(
                    "ERROR : An error occured while executing the last task. Please try again."
                );
            }
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddDbContext<DewitDbContext>(opts => opts.UseSqlite(DbConnectionString.Get()));

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            services.AddTransient<ITaskService, TaskService>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IDataConverter, DataConverterService>();
            services.AddTransient<IMoodService, MoodService>();

            services.AddTransient<App>();

            return services;
        }
    }
}