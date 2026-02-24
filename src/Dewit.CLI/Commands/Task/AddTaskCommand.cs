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
        private readonly Option<string?> _recurOpt;
        private readonly Option<string?> _descriptionOpt;

        public AddTaskCommand(ITaskService taskService, string name, string description)
            : base(name, description)
        {
            _taskService = taskService;

            _titleArg = new Argument<string>("title")
            {
                Description = "Short title for the task.",
            };
            _descriptionOpt = new Option<string?>("--description")
            {
                Description = "Additional context for the task (notes, links, etc.).",
            };
            _tagsOpt = new Option<string?>("--tags")
            {
                Description =
                    "Comma-separated list of tags to identify your task. Example : --tags work,testing",
            };
            _recurOpt = new Option<string?>("--recur")
            {
                Description =
                    "Set a recurrence schedule. Examples: daily, weekly, monthly, yearly, "
                    + "2d (every 2 days), 3w (every 3 weeks), 2m (every 2 months).",
            };

            Arguments.Add(_titleArg);
            Options.Add(_descriptionOpt);
            Options.Add(_tagsOpt);
            Options.Add(_recurOpt);

            SetAction(parseResult =>
            {
                var title = parseResult.GetValue(_titleArg)!;
                var description = parseResult.GetValue(_descriptionOpt);
                var tags = parseResult.GetValue(_tagsOpt);
                var recur = parseResult.GetValue(_recurOpt);
                AddTask(title, description, tags, recur);
            });
        }

        private void AddTask(string title, string? description = null, string? tags = null, string? recur = null)
        {
            try
            {
                var status = Name == "now" ? "Doing" : "Later";
                _taskService.AddTask(title, status, tags, recur, description);

                var recurMsg = recur != null ? $", [aqua]Recur[/] = {recur}" : "";
                Output.WriteVerbose(
                    $"Added a new task : {title}, Status = {status}, Tags = {tags}"
                );
                Output.WriteText(
                    $"[green]Added a new task[/] : {title}, [aqua]Status[/] = {status}, [aqua]Tags[/] = {tags}{recurMsg}"
                );
            }
            catch (ArgumentException ex)
            {
                Output.WriteVerbose(ex, "Invalid argument adding task");
                Output.WriteError(ex.Message);
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Failed to add task");
                Output.WriteError($"Failed to add task. Please try again.");
            }
        }
    }
}