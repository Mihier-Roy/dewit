using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Dewit.CLI.Data;
using Serilog;

namespace Dewit.CLI.Commands
{
	public class GetTasksCommand : Command
	{
		private readonly ITaskRepository _repository;

		public GetTasksCommand(ITaskRepository repository, string name, string description = null) : base(name, description)
		{
			Handler = CommandHandler.Create<string>(GetTasks);
			_repository = repository;
		}

		private void GetTasks(string obj)
		{
			Log.Debug("Showing all tasks");
			var tasks = _repository.GetTasks();
			foreach (var task in tasks)
			{
				Console.WriteLine($"[{task.AddedOn}] {task.Id} | {task.Status} | {task.TaskDescription}");
			}
		}
	}
}