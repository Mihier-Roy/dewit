using System.ComponentModel;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Dewit.CLI.Branches.Task
{
	public class GetTasksCommand : Command<GetTasksCommand.Settings>
	{
		public class Settings : CommandSettings
		{
			[CommandOption("--sort [sort-order]")]
			[Description("Sort tasks by status or date. Default is <date>")]
			public string? Sort { get; set; }
			
			[CommandOption("--duration [duration]")]
			[Description("Show tasks between the specified duration. Default is set to <today>")]
			public string? Duration { get; set; }
			
			[CommandOption("--status [status]")]
			[Description("Show tasks of specified status")]
			public string? Status { get; set; }
			
			[CommandOption("--tags [tags]")]
			[Description("Filter tasks based on tags")]
			public string? Tags { get; set; }
			
			[CommandOption("--search [search-text]")]
			[Description("Search for tasks that contain the input string")]
			public string? Search { get; set; }

			public override ValidationResult Validate()
			{
				string[] sortOptions = { "date", "status" };
				string[] durationOptions = { "all", "yesterday", "today", "week", "month" };
				string[] statusOptions = { "doing", "done", "later" };

				if (!sortOptions.Contains(Sort))
					ValidationResult.Error("Invalid sort option. Choose from: [date, status]");
				
				if(!durationOptions.Contains(Duration))
					ValidationResult.Error("Invalid duration option. Choose from: [all, yesterday, today, week, month]");
				
				if(!statusOptions.Contains(Status))
					ValidationResult.Error("Invalid status option. Choose from: [doing, done, later]");
				
				return ValidationResult.Success();
			}
		}
		
		private readonly ITaskService _taskService;

		public GetTasksCommand(ITaskService taskService)
		{
			_taskService = taskService;
		}
 
		public override int Execute(CommandContext context, Settings settings)
		{
			var tasks = _taskService.GetTasks(settings.Sort, settings.Duration, settings.Status, settings.Tags, settings.Search);
			Output.WriteText(
					$"Displaying tasks using parameters -> [aqua]sort[/]: {settings.Sort}, [aqua]duration[/] : {settings.Duration}, [aqua]status[/]: {settings.Status ?? "n/a"}, [aqua]tags[/]:{settings.Tags}");
			Output.WriteTasksTable(new[] { "ID", "Task", "Status", "Tags", "Added On", "Completed On" }, tasks);

			return 0;
		}
	}
}