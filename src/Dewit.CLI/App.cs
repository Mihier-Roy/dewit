using System;
using System.CommandLine;
using Dewit.CLI.Commands.Config;
using Dewit.CLI.Commands.Mood;
using Dewit.CLI.Commands.Task;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;

namespace Dewit.CLI
{
    public class App
    {
        private readonly ITaskService _taskService;
        private readonly IDataConverter _dataConverter;
        private readonly IMoodService _moodService;
        private readonly IConfigurationService _configService;

        public App(
            ITaskService taskService,
            IDataConverter dataConverter,
            IMoodService moodService,
            IConfigurationService configService
        )
        {
            _taskService = taskService;
            _dataConverter = dataConverter;
            _moodService = moodService;
            _configService = configService;
        }

        public void Run(string[] args)
        {
            var verboseOption = new Option<bool>("--verbose")
            {
                Description = "Show verbose output.",
                Recursive = true,
            };

            var rootCommand = new RootCommand("dewit")
            {
                new AddTaskCommand(_taskService, "now", "Create a new task and set it to 'Doing'"),
                new AddTaskCommand(
                    _taskService,
                    "later",
                    "Create a new task and set it to 'Later'"
                ),
                new TaskCommand(_taskService, _dataConverter, _configService),
                new MoodCommand(_moodService),
                new ConfigCommand(_configService, _moodService),
            };
            rootCommand.Options.Add(verboseOption);

            Output.IsVerbose = Array.IndexOf(args, "--verbose") >= 0;
            rootCommand.Parse(args).Invoke();
        }
    }
}
