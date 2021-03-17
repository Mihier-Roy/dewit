using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Dewit.CLI.Data;
using Dewit.CLI.Models;
using Serilog;

namespace Commands
{
	public class NowCommand : Command
	{
		private readonly ITaskRepository _repository;

		public NowCommand(ITaskRepository repository, string name, string description = null) : base(name, description)
		{
			AddArgument(new Argument<string>("name", "Task you're currently performing"));
			Handler = CommandHandler.Create<string>(AddTask);
			_repository = repository;
		}

		private void AddTask(string name)
		{
			Log.Debug($"Adding a new task : {name}, Status = now.");
			var newTask = new TaskItem
			{
				TaskDescription = name,
				AddedOn = DateTime.Now,
				Status = "now"
			};

			_repository.AddTask(newTask);
			var success = _repository.SaveChanges();

			if (success)
			{
				Log.Debug($"Added a new task : {name}, Status = now.");
				Console.WriteLine($"Added a new task : {name}, Status = now.");
			}
			else
			{
				Log.Error($"Failed to add task.");
				Console.WriteLine($"ERROR : Failed to add task. Please try again.");
			}
		}
	}
}