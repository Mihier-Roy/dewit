using System;
using System.CommandLine;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Task
{
    public class DeleteTaskCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly Argument<int> _idArg;

        public DeleteTaskCommand(ITaskService taskService)
            : base("delete", "Delete a task from your list")
        {
            _taskService = taskService;

            _idArg = new Argument<int>("id") { Description = "ID of the task to be deleted." };
            Arguments.Add(_idArg);

            SetAction(parseResult =>
            {
                var id = parseResult.GetValue(_idArg);
                DeleteTask(id);
            });
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
                    Output.WriteVerbose($"Task with ID {id} does not exist.");
                    Output.WriteError(
                        $"Task with ID {id} does not exist. View all tasks with -> dewit list"
                    );
                    return;
                }

                var taskDescription = task.TaskDescription;
                _taskService.DeleteTask(id);

                Output.WriteVerbose($"Deleted task : {id} | {taskDescription}");
                Output.WriteText($"[yellow]Deleted task[/] : {id} | {taskDescription}");
            }
            catch (ApplicationException ex)
            {
                Output.WriteVerbose(ex, "Failed to delete task");
                Output.WriteError(ex.Message);
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Unexpected error deleting task");
                Output.WriteError("Failed to delete. Please try again.");
            }
        }
    }
}
