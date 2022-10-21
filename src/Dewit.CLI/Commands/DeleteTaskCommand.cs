using System.CommandLine;
using System.CommandLine.Invocation;

namespace Dewit.CLI.Commands
{
	public class DeleteTaskCommand : Command
	{
		public DeleteTaskCommand(string name, string? description = null) : base(name, description)
		{
			AddArgument(new Argument<int>("id", "ID of the task to be deleted."));
			Handler = CommandHandler.Create<int>(DeleteTask);
		}

		// Output.WriteError($"Task with ID {id} does not exist. View all tasks with -> dewit list");
		// Output.WriteText($"[yellow]Deleted task[/] : {task.Id} | {task.TaskDescription} ");
		// Output.WriteError($"Failed to delete. Please try again.");
	}
}