using System.CommandLine;
using Dewit.CLI.Commands;
using Dewit.CLI.Commands.Config;
using Dewit.CLI.Commands.Mood;
using Dewit.Core.Interfaces;

namespace Dewit.CLI
{
    public class App
    {
        private readonly ITaskService _taskService;
        private readonly IDataConverter _dataConverter;
        private readonly IMoodService _moodService;
        private readonly IConfigurationService _configService;

        public App(ITaskService taskService, IDataConverter dataConverter, IMoodService moodService, IConfigurationService configService)
        {
            _taskService = taskService;
            _dataConverter = dataConverter;
            _moodService = moodService;
            _configService = configService;
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
                new ExportTasksCommand(_taskService, _dataConverter, _configService, "export"),
                new ImportTasksCommand(_taskService, _dataConverter, "import"),
                new MoodCommand(_moodService),
                new ConfigCommand(_configService, _moodService),
            };

            rootCommand.Parse(args).Invoke();
        }
    }
}
