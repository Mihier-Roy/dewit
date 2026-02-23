using System;
using System.CommandLine;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Task
{
    public class UpdateTaskCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly Argument<int> _idArg;
        private readonly Option<string?> _titleOpt;
        private readonly Option<string?> _addTagsOpt;
        private readonly Option<string?> _removeTagsOpt;
        private readonly Option<bool> _resetTagsOpt;
        private readonly Option<string?> _recurOpt;
        private readonly Option<bool> _removeRecurOpt;

        public UpdateTaskCommand(ITaskService taskService)
            : base("edit", "Edit an existing task")
        {
            _taskService = taskService;

            _idArg = new Argument<int>("id") { Description = "ID of the task you wish to update." };
            _titleOpt = new Option<string?>("--title")
            {
                Description = "Change the description of the task.",
            };
            _addTagsOpt = new Option<string?>("--add-tags")
            {
                Description = "Add new tag(s) to an existing task. Example --add-tags work,test",
            };
            _removeTagsOpt = new Option<string?>("--remove-tags")
            {
                Description =
                    "Remove tag(s) from an existing task. Example --remove-tags work,test",
            };
            _resetTagsOpt = new Option<bool>("--reset-tags")
            {
                Description = "Remove all tags from an existing task.",
            };
            _recurOpt = new Option<string?>("--recur")
            {
                Description =
                    "Assign or change the recurrence schedule. "
                    + "Examples: daily, weekly, monthly, yearly, 2d, 3w, 2m.",
            };
            _removeRecurOpt = new Option<bool>("--remove-recur")
            {
                Description = "Remove the recurrence schedule from this task.",
            };

            Arguments.Add(_idArg);
            Options.Add(_titleOpt);
            Options.Add(_addTagsOpt);
            Options.Add(_removeTagsOpt);
            Options.Add(_resetTagsOpt);
            Options.Add(_recurOpt);
            Options.Add(_removeRecurOpt);

            SetAction(parseResult =>
            {
                var id = parseResult.GetValue(_idArg);
                var title = parseResult.GetValue(_titleOpt);
                var addTags = parseResult.GetValue(_addTagsOpt);
                var removeTags = parseResult.GetValue(_removeTagsOpt);
                var resetTags = parseResult.GetValue(_resetTagsOpt);
                var recur = parseResult.GetValue(_recurOpt);
                var removeRecur = parseResult.GetValue(_removeRecurOpt);
                UpdateTaskDetails(id, title, addTags, removeTags, resetTags, recur, removeRecur);
            });
        }

        private void UpdateTaskDetails(
            int id,
            string? title = null,
            string? addTags = null,
            string? removeTags = null,
            bool resetTags = false,
            string? recur = null,
            bool removeRecur = false
        )
        {
            try
            {
                var task = _taskService.UpdateTaskDetails(
                    id,
                    title,
                    addTags,
                    removeTags,
                    resetTags,
                    recur,
                    removeRecur
                );

                var recurInfo =
                    task.RecurringSchedule != null
                        ? $" | [blue]Recur[/] = {task.RecurringSchedule.ToLabel()}"
                        : "";

                Output.WriteVerbose(
                    $"Successfully updated task : {task.Id} | {task.TaskDescription} | {task.Tags}"
                );
                Output.WriteText(
                    $"[green]Successfully updated task[/] : {task.Id} | {task.TaskDescription} | {task.Tags}{recurInfo}"
                );
            }
            catch (ArgumentException ex)
            {
                Output.WriteVerbose(ex, "Invalid argument updating task");
                Output.WriteError(ex.Message);
            }
            catch (ApplicationException ex)
            {
                Output.WriteVerbose(ex, "Failed to update task");
                Output.WriteError(ex.Message);
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Unexpected error updating task");
                Output.WriteError("Failed to update task details. Please try again.");
            }
        }
    }
}