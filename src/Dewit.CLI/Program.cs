using System.IO;
using Dewit.CLI.Branches.Config;
using Dewit.CLI.Branches.DataTransfer;
using Dewit.CLI.Branches.Setup;
using Dewit.CLI.Branches.Task;
using Dewit.Core.Interfaces;
using Dewit.Core.Services;
using Dewit.Infrastructure.Data;
using Dewit.Infrastructure.Data.Repositories;
using Dewit.CLI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Spectre.Console.Cli;

namespace Dewit.CLI
{
    class Program
    {
        public static int Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            var config = configurationBuilder.Build();
            var app = new CommandApp(RegisterServices(config));
            app.Configure(config => ConfigureCommands(config));

            if (!File.Exists(config.GetValue<string>("ConnectionStrings:Sqlite").Split('=')[1]))
                return app.Run(new[] { "setup", "--first-run" });

            return app.Run(args);
        }

        private static ITypeRegistrar RegisterServices(IConfiguration config)
        {
            var services = new ServiceCollection();

            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<ITaskService, TaskService>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddDbContext<DewitDbContext>(opts => opts.UseSqlite(config.GetConnectionString("Sqlite")));
            services.AddLogging(builder =>
            {
                var logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .ReadFrom.Configuration(config)
                    .Enrich.FromLogContext()
                    .WriteTo.File("logs/dewit.log",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                builder.AddSerilog(logger);
            });

            return new TypeRegistrar(services);
        }

        private static IConfigurator ConfigureCommands(IConfigurator config)
        {
            config.CaseSensitivity(CaseSensitivity.None);
            config.SetApplicationName("dewit");
            config.ValidateExamples();

            config.AddBranch("task", task =>
            {
                task.SetDescription("View, list, add or remove tasks.");

                task.AddCommand<AddTaskCommand>("now")
                    .WithAlias("later")
                    .WithDescription("Add a new task")
                    .WithExample(new[]
                        { "task", "now", "Write a new task", "--tags", "test,tags,now" });

                task.AddCommand<CompleteTaskCommand>("done")
                    .WithDescription("Complete a task")
                    .WithExample(new[] { "task", "done", "1001" });

                task.AddCommand<UpdateTaskCommand>("edit")
                    .WithDescription("Update a task")
                    .WithExample(new[] { "task", "edit", "1001" });

                task.AddCommand<GetTasksCommand>("list")
                    .WithDescription("View and filter tasks")
                    .WithExample(new[] { "task", "list" });

                task.AddCommand<DeleteTaskCommand>("delete")
                    .WithDescription("Delete a task")
                    .WithExample(new[] { "task", "delete", "1" });
            });

            config.AddCommand<ImportTasksCommand>("import")
                .WithDescription("Import task data")
                .WithExample(new[] { "import", "./test.json" });

            config.AddCommand<ExportTasksCommand>("export")
                .WithDescription("Export task data")
                .WithExample(new[] { "export", "./test.json" });

            config.AddCommand<ConfigCommand>("config")
                .WithDescription("View and update the settings of the application");
            
            config.AddCommand<SetupCommand>("setup")
                .WithDescription("Setup the application for first time use");

            return config;
        }
    }
}