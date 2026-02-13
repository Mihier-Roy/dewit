using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class UpdateStatusCommand : Command
    {
        private readonly ITaskService _taskService;

        public UpdateStatusCommand(ITaskService taskService, string name, string? description = null) : base(name, description)
        {
            AddArgument(new Argument<int>("id", "ID of the task you wish to update."));
            AddOption(new Option<string>("--completed-at", "Specify when the task was completed"));
            Handler = CommandHandler.Create<int, string>(UpdateStatus);
            _taskService = taskService;
        }

        private void UpdateStatus(int id, string completedAt)
        {
            try
            {
                _taskService.CompleteTask(id, completedAt ?? string.Empty);

                // Get the task to display its details
                var tasks = _taskService.GetTasks(duration: "all");
                var task = tasks.FirstOrDefault(t => t.Id == id);

                if (task != null)
                {
                    Log.Information($"Completed task : {task.Id} | {task.TaskDescription}");
                    Output.WriteText($"[green]Completed task[/] : {task.Id} | {task.TaskDescription}");
                }
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex, "Failed to complete task");
                Output.WriteError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Log.Error(ex, "Invalid completion date");
                Output.WriteError("Invalid date format. Please try again.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error");
                Output.WriteError("Failed to set task as completed. Please try again.");
            }
        }
    }
}
