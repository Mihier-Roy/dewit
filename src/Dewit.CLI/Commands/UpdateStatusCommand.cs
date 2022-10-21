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
	}
}