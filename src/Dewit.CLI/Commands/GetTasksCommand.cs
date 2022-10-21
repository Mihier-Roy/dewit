using System.CommandLine;
using System.CommandLine.Invocation;

namespace Dewit.CLI.Commands
{
	public class GetTasksCommand : Command
	{
		public GetTasksCommand(string name, string? description = null) : base(name, description)
		{
			var sortOptions = new Option<string>("--sort", "Sort tasks by status or date. Default is <date>")
									.FromAmong("status", "date");
			var durationOptions = new Option<string>("--duration", "Show tasks between the specified duration. Default is set to <today>.")
									.FromAmong("all", "yesterday", "today", "week", "month");
			var statusOptions = new Option<string>("--status", "Show tasks of specified status.")
									.FromAmong("doing", "done", "later");
			var tagOptions = new Option<string>("--tags", "Filter tasks based on tags.");
			var searchOptions = new Option<string>("--search", "Search for tasks that contain the input string.");
			AddOption(sortOptions);
			AddOption(durationOptions);
			AddOption(statusOptions);
			AddOption(tagOptions);
			AddOption(searchOptions);
			Handler = CommandHandler.Create<string, string, string, string, string>(GetTasks);
		}

		// Output.WriteText($"Displaying tasks using parameters -> [aqua]sort[/]: {sort}, [aqua]duration[/] : {duration}, [aqua]status[/]: {status ?? "n/a"}, [aqua]tags[/]:{tags}");
		// Output.WriteTable(new string[] { "ID", "Task", "Status", "Tags", "Added On", "Completed On" }, tasks);
	}
}