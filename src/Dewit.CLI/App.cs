using System.CommandLine;
using Dewit.CLI.Commands;
using Dewit.CLI.Data;
using Dewit.Core.Interfaces;

namespace Dewit.CLI
{
    public class App
    {
        private readonly ITaskRepository _repository;
        private readonly ITaskService _taskService;

        public App(ITaskRepository repository, ITaskService taskService)
        {
            _repository = repository;
            _taskService = taskService;
        }

        public void Run(string[] args)
        {
            var rootCommand = new RootCommand("dewit"){
                new AddTaskCommand(_taskService, "now"),
                new AddTaskCommand(_taskService, "later"),
                new UpdateStatusCommand(_repository, "done"),
                new UpdateTaskCommand(_repository, "edit"),
                new GetTasksCommand(_repository, "list"),
                new DeleteTaskCommand(_repository, "delete"),
                new ExportTasksCommand(_repository, "export"),
                new ImportTasksCommand(_repository, "import"),
            };

            rootCommand.InvokeAsync(args).Wait();
        }
    }
}
