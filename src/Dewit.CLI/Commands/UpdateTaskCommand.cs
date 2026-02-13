using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class UpdateTaskCommand : Command
    {
        private readonly ITaskService _taskService;

        public UpdateTaskCommand(ITaskService taskService, string name, string? description = null) : base(name, description)
        {
            AddArgument(new Argument<int>("id", "ID of the task you wish to update."));
            AddOption(new Option<string>("--title", "Change the description of the task."));
            AddOption(new Option<string>("--add-tags", "Add new tag(s) to an existing task. Example --add-tags work,test"));
            AddOption(new Option<string>("--remove-tags", "Remove tag(s) from an existing task. Example --remove-tags work,test"));
            AddOption(new Option<bool>("--reset-tags", "Remove all tags from an existing task."));
            Handler = CommandHandler.Create<int, string, string, string, bool>(UpdateTaskDetails);
            _taskService = taskService;
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
