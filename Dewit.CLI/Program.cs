using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dewit.CLI
{
	class Program
	{
		static void Main(string[] args)
		{
			var services = ConfigureServices();
			var serviceProvider = services.BuildServiceProvider();

			serviceProvider.GetService<App>().Run(args);
		}

		private static IServiceCollection ConfigureServices()
		{
			IServiceCollection services = new ServiceCollection();

			// Make config available throughout the application
			var config = LoadConfiguration();
			services.AddSingleton(config);

			// required to run the application
			services.AddTransient<App>();

			return services;
		}

		private static IConfiguration LoadConfiguration()
		{
			var builder = new ConfigurationBuilder()
								.SetBasePath(Directory.GetCurrentDirectory())
								.AddJsonFile("config.json");
			return builder.Build();
		}
	}
}
