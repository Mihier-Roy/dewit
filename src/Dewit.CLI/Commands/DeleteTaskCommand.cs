using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class DeleteTaskCommand : Command
    {
        private readonly ITaskService _taskService;

        public DeleteTaskCommand(ITaskService taskService, string name, string? description = null) : base(name, description)
        {
            AddArgument(new Argument<int>("id", "ID of the task to be deleted."));
            Handler = CommandHandler.Create<int>(DeleteTask);
            _taskService = taskService;
        }

        private void DeleteTask(int id)
        {
            try
            {
                // Get task details before deleting for display purposes
                var tasks = _taskService.GetTasks(duration: "all");
                var task = tasks.FirstOrDefault(t => t.Id == id);

                if (task == null)
                {
                    Log.Error($"Task with ID {id} does not exist.");
                    Output.WriteError($"Task with ID {id} does not exist. View all tasks with -> dewit list");
                    return;
                }

                var taskDescription = task.TaskDescription;
                _taskService.DeleteTask(id);

                Log.Information($"Deleted task : {id} | {taskDescription}");
                Output.WriteText($"[yellow]Deleted task[/] : {id} | {taskDescription}");
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex, "Failed to delete task");
                Output.WriteError(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error");
                Output.WriteError("Failed to delete. Please try again.");
            }
        }
    }
}
