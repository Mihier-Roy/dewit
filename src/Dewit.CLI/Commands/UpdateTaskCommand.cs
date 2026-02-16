using System;
using System.CommandLine;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class UpdateTaskCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly Argument<int> _idArg;
        private readonly Option<string?> _titleOpt;
        private readonly Option<string?> _addTagsOpt;
        private readonly Option<string?> _removeTagsOpt;
        private readonly Option<bool> _resetTagsOpt;

        public UpdateTaskCommand(ITaskService taskService, string name, string? description = null) : base(name, description)
        {
            _taskService = taskService;

            _idArg = new Argument<int>("id")
            {
                Description = "ID of the task you wish to update."
            };
            _titleOpt = new Option<string?>("--title")
            {
                Description = "Change the description of the task."
            };
            _addTagsOpt = new Option<string?>("--add-tags")
            {
                Description = "Add new tag(s) to an existing task. Example --add-tags work,test"
            };
            _removeTagsOpt = new Option<string?>("--remove-tags")
            {
                Description = "Remove tag(s) from an existing task. Example --remove-tags work,test"
            };
            _resetTagsOpt = new Option<bool>("--reset-tags")
            {
                Description = "Remove all tags from an existing task."
            };

            this.Arguments.Add(_idArg);
            this.Options.Add(_titleOpt);
            this.Options.Add(_addTagsOpt);
            this.Options.Add(_removeTagsOpt);
            this.Options.Add(_resetTagsOpt);

            this.SetAction(parseResult =>
            {
                var id = parseResult.GetValue(_idArg);
                var title = parseResult.GetValue(_titleOpt);
                var addTags = parseResult.GetValue(_addTagsOpt);
                var removeTags = parseResult.GetValue(_removeTagsOpt);
                var resetTags = parseResult.GetValue(_resetTagsOpt);
                UpdateTaskDetails(id, title, addTags, removeTags, resetTags);
            });
        }

        private void UpdateTaskDetails(int id, string? title = null, string? addTags = null, string? removeTags = null, bool resetTags = false)
        {
            try
            {
                var task = _taskService.UpdateTaskDetails(id, title, addTags, removeTags, resetTags);

                Log.Information($"Successfully updated task : {task.Id} | {task.TaskDescription} | {task.Tags}");
                Output.WriteText($"[green]Successfully updated task[/] : {task.Id} | {task.TaskDescription} | {task.Tags}");
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex, "Failed to update task");
                Output.WriteError(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error");
                Output.WriteError("Failed to update task details. Please try again.");
            }
        }
    }
}
