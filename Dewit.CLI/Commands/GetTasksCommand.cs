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
			var sortOptions = new Option<string>("--sort", "Sort tasks by status or date").FromAmong("status", "date");
			AddOption(sortOptions);
			Handler = CommandHandler.Create<string>(GetTasks);
			_repository = repository;
		}

		private void GetTasks(string sort)
		{
			Log.Debug("Showing all tasks");
			var tasks = _repository.GetTasks().Where(p => p.AddedOn > DateTime.Today.AddDays(-1));

			if (sort == "status")
				tasks = tasks.OrderBy(p => p.Status);
			else
				tasks = tasks.OrderBy(p => p.AddedOn);


			Output.WriteTable(new string[] { "ID", "Task", "Status", "AddedOn", "CompletedOn" }, tasks);
		}
	}
}