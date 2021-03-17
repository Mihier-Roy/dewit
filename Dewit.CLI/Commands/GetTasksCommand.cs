using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using Dewit.CLI.Data;
using Dewit.CLI.Utils;
using Serilog;

namespace Dewit.CLI.Commands
{
	public class GetTasksCommand : Command
	{
		private readonly ITaskRepository _repository;

		public GetTasksCommand(ITaskRepository repository, string name, string description = null) : base(name, description)
		{
			var sortOptions = new Option<string>("--sort", "Sort tasks by status or date. Default is <date>")
									.FromAmong("status", "date");
			var durationOptions = new Option<string>("--duration", "Show tasks between the specified duration. Default is set to <today>.")
									.FromAmong("all", "yesterday", "today", "week", "month");
			var statusOptions = new Option<string>("--status", "Show tasks of specified status.")
									.FromAmong("doing", "done", "later");
			AddOption(sortOptions);
			AddOption(durationOptions);
			AddOption(statusOptions);
			Handler = CommandHandler.Create<string, string, string>(GetTasks);
			_repository = repository;
		}

		private void GetTasks(string sort = "date", string duration = "all", string status = null)
		{
			Log.Debug($"Showing all tasks with arguments -> sort: {sort}, duration : {duration}, status: {status}");
			var tasks = _repository.GetTasks();

			switch (duration)
			{
				case "yesterday":
					tasks = tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-1));
					break;
				case "today":
					tasks = tasks.Where(p => p.AddedOn.Date == DateTime.Today.Date);
					break;
				case "week":
					tasks = tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-7));
					break;
				case "month":
					tasks = tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-30));
					break;
				case "all": break;
			}

			switch (status)
			{
				case "doing":
					tasks = tasks.Where(p => p.Status == "Doing");
					break;
				case "done":
					tasks = tasks.Where(p => p.Status == "Done");
					break;
				case "later":
					tasks = tasks.Where(p => p.Status == "Later");
					break;
			}

			if (sort == "status")
				tasks = tasks.OrderBy(p => p.Status);
			else
				tasks = tasks.OrderBy(p => p.AddedOn);

			Output.WriteText($"Displaying tasks using parameters -> [aqua]sort[/]: {sort}, [aqua]duration[/] : {duration}, [aqua]status[/]: {(status == null ? "n/a" : status)}");
			Output.WriteTable(new string[] { "ID", "Task", "Status", "Tags", "AddedOn", "CompletedOn" }, tasks);
		}
	}
}