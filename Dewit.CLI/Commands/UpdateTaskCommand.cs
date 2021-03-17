using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using Dewit.CLI.Data;
using Dewit.CLI.Utils;
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
				Log.Error($"Task with ID {id} does not exist.");
				Output.WriteError($"Task with ID {id} does not exist. View all tasks with -> dewit list");
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
				{
					Log.Error($"Failed to set status of task [{id}] to Done");
					Output.WriteError("Failed to set task as completed. Please try again.");
				}
			}
			else
				task.CompletedOn = DateTime.Now;

			task.Status = "Done";
			_repository.UpdateTask(task);
			var success = _repository.SaveChanges();

			if (success)
			{
				Log.Information($"Completed task : {task.Id} | {task.TaskDescription} ");
				Output.WriteText($"[green]Completed task[/] : {task.Id} | {task.TaskDescription} ");
			}
			else
			{
				Log.Error($"Failed to set status of task [{id}] to Done");
				Output.WriteError($"Failed to set task as completed. Please try again.");
			}
		}
	}
}