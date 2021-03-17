using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Dewit.CLI.Data;
using Serilog;

namespace Dewit.CLI.Commands
{
	public class UpdateTaskCommand : Command
	{
		private readonly ITaskRepository _repository;

		public UpdateTaskCommand(ITaskRepository repository, string name, string description = null) : base(name, description)
		{
			AddArgument(new Argument<int>("id", "ID of the task you wish to update."));
			Handler = CommandHandler.Create<int>(UpdateTask);
			_repository = repository;
		}

		private void UpdateTask(int id)
		{
			Log.Debug($"Setting status of task [{id}] to Done");

			var task = _repository.GetTaskById(id);
			task.CompletedOn = DateTime.Now;
			task.Status = "Done";

			_repository.UpdateTask(task);
			var success = _repository.SaveChanges();

			if (success)
				Console.WriteLine($"Completed task : {task.Id} | {task.TaskDescription} ");
			else
				Console.WriteLine($"ERROR : Failed to complete task. Please try again.");
		}
	}
}