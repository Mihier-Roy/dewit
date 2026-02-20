using System;
using System.CommandLine;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class AddTaskCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly string _name;
        private readonly Argument<string> _titleArg;
        private readonly Option<string?> _tagsOpt;

        public AddTaskCommand(ITaskService taskService, string name, string? description = null)
            : base(name, description)
        {
            _taskService = taskService;
            _name = name;

            _titleArg = new Argument<string>("title")
            {
                Description = "Description of the task you're currently performing.",
            };
            _tagsOpt = new Option<string?>("--tags")
            {
                Description =
                    "Comma-separated list of tags to identify your task. Example : --tags work,testing",
            };

            this.Arguments.Add(_titleArg);
            this.Options.Add(_tagsOpt);

            this.SetAction(parseResult =>
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
                var status = _name == "now" ? "Doing" : "Later";
                _taskService.AddTask(title, status, tags);

                Log.Information($"Added a new task : {title}, Status = {status}, Tags = {tags}");
                Output.WriteText(
                    $"[green]Added a new task[/] : {title}, [aqua]Status[/] = {status}, [aqua]Tags[/] = {tags}"
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add task");
                Output.WriteError($"Failed to add task. Please try again.");
            }
        }
    }
}
