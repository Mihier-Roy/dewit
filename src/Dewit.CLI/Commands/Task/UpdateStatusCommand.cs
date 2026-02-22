using System;
using System.CommandLine;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Task
{
    public class UpdateStatusCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly Argument<int> _idArg;
        private readonly Option<string?> _completedAtOpt;

        public UpdateStatusCommand(ITaskService taskService)
            : base("done", "Complete a task")
        {
            _taskService = taskService;

            _idArg = new Argument<int>("id") { Description = "ID of the task you wish to update." };
            _completedAtOpt = new Option<string?>("--completed-at")
            {
                Description = "Specify when the task was completed",
            };

            Arguments.Add(_idArg);
            Options.Add(_completedAtOpt);

            SetAction(parseResult =>
            {
                var id = parseResult.GetValue(_idArg);
                var completedAt = parseResult.GetValue(_completedAtOpt);
                UpdateStatus(id, completedAt);
            });
        }

        private void UpdateStatus(int id, string? completedAt)
        {
            try
            {
                _taskService.CompleteTask(id, completedAt ?? string.Empty);

                // Get the task to display its details
                var tasks = _taskService.GetTasks(duration: "all");
                var task = tasks.FirstOrDefault(t => t.Id == id);

                if (task != null)
                {
                    Output.WriteVerbose($"Completed task : {task.Id} | {task.TaskDescription}");
                    Output.WriteText(
                        $"[green]Completed task[/] : {task.Id} | {task.TaskDescription}"
                    );
                }
            }
            catch (ApplicationException ex)
            {
                Output.WriteVerbose(ex, "Failed to complete task");
                Output.WriteError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Output.WriteVerbose(ex, "Invalid completion date");
                Output.WriteError("Invalid date format. Please try again.");
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Unexpected error completing task");
                Output.WriteError("Failed to set task as completed. Please try again.");
            }
        }
    }
}