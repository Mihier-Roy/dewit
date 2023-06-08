using Dewit.CLI.Branches.DataTransfer;
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
using ExportTasksCommand = Dewit.CLI.Branches.DataTransfer.ExportTasksCommand;

namespace Dewit.CLI
{
    class Program
    {
        public static int Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            var app = new CommandApp(RegisterServices(configuration.Build()));
            app.Configure(config => ConfigureCommands(config));

            return app.Run(args);
        }

        private static ITypeRegistrar RegisterServices(IConfiguration config)
        {
            var services = new ServiceCollection();

            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<ITaskService, TaskService>();
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
                .WithDescription("View list of students.")
                .WithExample(new[] { "import", "./test.json" });
            
            config.AddCommand<ExportTasksCommand>("export")
                .WithDescription("View list of students.")
                .WithExample(new[] { "export", "./test.json" });

            return config;
        }
    }
}