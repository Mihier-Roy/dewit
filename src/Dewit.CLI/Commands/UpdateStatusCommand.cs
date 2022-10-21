using System.CommandLine;
using System.CommandLine.Invocation;

namespace Dewit.CLI.Commands
{
	public class UpdateStatusCommand : Command
	{
		public UpdateStatusCommand(string name, string? description = null) : base(name, description)
		{
			AddArgument(new Argument<int>("id", "ID of the task you wish to update."));
			AddOption(new Option<string>("--completed-at", "Specify when the task was completed"));
			Handler = CommandHandler.Create<int, string>(UpdateStatus);
		}
		// Output.WriteError($"Task with ID {id} does not exist. View all tasks with -> dewit list");
		// Output.WriteError("Failed to set task as completed. Please try again.");
		// Output.WriteText($"[green]Completed task[/] : {task.Id} | {task.TaskDescription} ");
		// Output.WriteError($"Failed to set task as completed. Please try again.");
	}
}