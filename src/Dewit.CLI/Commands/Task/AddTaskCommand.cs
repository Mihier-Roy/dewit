using System;
using System.CommandLine;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Task
{
    public class AddTaskCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly Argument<string> _titleArg;
        private readonly Option<string?> _tagsOpt;

        public AddTaskCommand(ITaskService taskService, string name, string description)
            : base(name, description)
        {
            _taskService = taskService;

            _titleArg = new Argument<string>("title")
            {
                Description = "Description of the task you're currently performing.",
            };
            _tagsOpt = new Option<string?>("--tags")
            {
                Description =
                    "Comma-separated list of tags to identify your task. Example : --tags work,testing",
            };

            Arguments.Add(_titleArg);
            Options.Add(_tagsOpt);

            SetAction(parseResult =>
            {
                var title = parseResult.GetValue(_titleArg)!;
                var tags = parseResult.GetValue(_tagsOpt);
                AddTask(title, tags);
            });
        }

        private void AddTask(string title, string? tags = null)
        {
            try
            {
                var status = Name == "now" ? "Doing" : "Later";
                _taskService.AddTask(title, status, tags);

                Output.WriteVerbose($"Added a new task : {title}, Status = {status}, Tags = {tags}");
                Output.WriteText(
                    $"[green]Added a new task[/] : {title}, [aqua]Status[/] = {status}, [aqua]Tags[/] = {tags}"
                );
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Failed to add task");
                Output.WriteError($"Failed to add task. Please try again.");
            }
        }
    }
}
