using System.CommandLine;
using Dewit.CLI.Commands;
using Dewit.Core.Interfaces;

namespace Dewit.CLI
{
    public class App
    {
        private readonly ITaskService _taskService;

        public App(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public void Run(string[] args)
        {
            var rootCommand = new RootCommand("dewit"){
                new AddTaskCommand(_taskService, "now"),
                new AddTaskCommand(_taskService, "later"),
                new UpdateStatusCommand(_taskService, "done"),
                new UpdateTaskCommand(_taskService, "edit"),
                new GetTasksCommand(_taskService, "list"),
                new DeleteTaskCommand(_taskService, "delete"),
                new ExportTasksCommand(_taskService, "export"),
                new ImportTasksCommand(_taskService, "import"),
            };

            rootCommand.InvokeAsync(args).Wait();
        }
    }
}
