using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
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
			AddOption(new Option<string>("--completed-at", "Specify when the task was completed"));
			Handler = CommandHandler.Create<int, string>(UpdateTask);
			_repository = repository;
		}

		private void UpdateTask(int id, string completedAt)
		{
			DateTime completedOn;
			Log.Debug($"Setting status of task [{id}] to Done");

			var task = _repository.GetTaskById(id);
			if (null == task)
			{
				Console.WriteLine($"ERROR: Task with ID {id} does not exist. View all tasks with -> dewit list");
				return;
			}

			// If the completed-at option is provided, parse the date entered by the user
			if (!string.IsNullOrEmpty(completedAt))
			{
				var culture = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
				var styles = DateTimeStyles.AssumeLocal;

				if (DateTime.TryParse(completedAt, culture, styles, out completedOn))
					task.CompletedOn = completedOn;
				else
					Console.WriteLine($"ERROR : Failed to complete task. Please try again.");
			}
			else
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