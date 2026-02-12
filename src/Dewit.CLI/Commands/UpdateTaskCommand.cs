using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using System.Linq;
using Dewit.CLI.Data;
using Dewit.CLI.Utils;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class UpdateTaskCommand : Command
    {
        private readonly ITaskRepository _repository;

        public UpdateTaskCommand(ITaskRepository repository, string name, string? description = null) : base(name, description)
        {
            AddArgument(new Argument<int>("id", "ID of the task you wish to update."));
            AddOption(new Option<string>("--title", "Change the description of the task."));
            AddOption(new Option<string>("--add-tags", "Add new tag(s) to an existing task. Example --add-tags work,test"));
            AddOption(new Option<string>("--remove-tags", "Remove tag(s) from an existing task. Example --remove-tags work,test"));
            AddOption(new Option<bool>("--reset-tags", "Remove all tags from an existing task."));
            Handler = CommandHandler.Create<int, string, string, string, bool>(UpdateTaskDetails);
            _repository = repository;
        }

        private void UpdateTaskDetails(int id, string? title = null, string? addTags = null, string? removeTags = null, bool resetTags = false)
        {
            Log.Debug($"Modifying information of task [{id}]. Params -> Title: {title}, Tags: {addTags}");

            var task = _repository.GetTaskById(id);
            if (null == task)
            {
                Log.Error($"Task with ID {id} does not exist.");
                Output.WriteError($"Task with ID {id} does not exist. View all tasks with -> dewit list");
                return;
            }

            // Modify the title of the task
            if (!string.IsNullOrEmpty(title))
                task.TaskDescription = title;

            // Add tag(s) to the task
            if (!string.IsNullOrEmpty(addTags))
            {
                addTags = Sanitizer.SanitizeTags(addTags);
                var updatedTags = string.Join(',', task.Tags, addTags);
                updatedTags = Sanitizer.DeduplicateTags(updatedTags);
                task.Tags = updatedTags[0] == ',' ? updatedTags.Remove(0, 1) : updatedTags;
            }

            // Remove tag(s) from a task
            if (!string.IsNullOrEmpty(removeTags))
            {
                var tagsToRemove = Sanitizer.SanitizeTags(removeTags).Split(',');
                var oldTags = task.Tags.Split(',');
                task.Tags = string.Join(',', oldTags.Except(tagsToRemove));
            }

            // Remove all tags from a task
            if (resetTags)
            {
                task.Tags = string.Empty;
            }

            _repository.UpdateTask(task);
            var success = _repository.SaveChanges();

            if (success)
            {
                Log.Information($"Successfully updated task : {task.Id} | {task.TaskDescription} | {task.Tags}");
                Output.WriteText($"[green]Successfully updated task[/] : {task.Id} | {task.TaskDescription} | {task.Tags}");
            }
            else
            {
                Log.Error($"Failed to update task [{id}].");
                Output.WriteError($"Failed to update task details. Please try again.");
            }
        }
    }
}
