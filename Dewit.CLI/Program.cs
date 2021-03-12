using System;
using System.IO;
using Dewit.CLI.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

				// Start the application
				serviceProvider.GetService<App>().Run(args);
			}
			catch (FileNotFoundException ex)
			{
				Log.Fatal(ex, "Unable to load config file. Exiting.");
				Console.WriteLine("ERROR : Unable to load config file. Please try again.");
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Something went wrong while starting the application.");
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

			// required to run the application
			services.AddTransient<App>();
			services.AddTransient<ITaskRepository, CsvTaskRepository>();

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
