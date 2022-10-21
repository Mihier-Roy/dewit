using System.CommandLine;
using System.CommandLine.Invocation;

namespace Dewit.CLI.Commands
{
	public class AddTaskCommand : Command
	{
		private readonly string _name;

		public AddTaskCommand(string name, string? description = null) : base(name, description)
		{
			AddArgument(new Argument<string>("title", "Description of the task you're currently performing."));
			AddOption(new Option<string>("--tags", "Comma-separated list of tags to identify your task. Example : --tags work,testing"));
			Handler = CommandHandler.Create<string, string>(AddTask);
			_name = name;
		}
	}
}