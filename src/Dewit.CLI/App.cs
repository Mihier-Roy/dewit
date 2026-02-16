using System.CommandLine;
using Dewit.CLI.Commands;
using Dewit.Core.Interfaces;

namespace Dewit.CLI
{
    public class App
    {
        private readonly ITaskService _taskService;
        private readonly IDataConverter _dataConverter;

        public App(ITaskService taskService, IDataConverter dataConverter)
        {
            _taskService = taskService;
            _dataConverter = dataConverter;
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
                new ExportTasksCommand(_taskService, _dataConverter, "export"),
                new ImportTasksCommand(_taskService, _dataConverter, "import"),
            };

            rootCommand.Parse(args).Invoke();
        }
    }
}
