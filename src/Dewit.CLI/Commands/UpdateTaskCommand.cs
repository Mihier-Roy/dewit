using System.CommandLine;
using System.CommandLine.Invocation;

namespace Dewit.CLI.Commands
{
	public class UpdateTaskCommand : Command
	{
		public UpdateTaskCommand(string name, string? description = null) : base(name, description)
		{
			AddArgument(new Argument<int>("id", "ID of the task you wish to update."));
			AddOption(new Option<string>("--title", "Change the description of the task."));
			AddOption(new Option<string>("--add-tags", "Add new tag(s) to an existing task. Example --add-tags work,test"));
			AddOption(new Option<string>("--remove-tags", "Remove tag(s) from an existing task. Example --remove-tags work,test"));
			AddOption(new Option<bool>("--reset-tags", "Remove all tags from an existing task."));
			Handler = CommandHandler.Create<int, string, string, string, bool>(UpdateTaskDetails);
		}
	}
}