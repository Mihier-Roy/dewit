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
			AddOption(new Option<string>("--title", "Change the description of the task."));
			AddOption(new Option<string>("--add-tags", "Add new tag(s) to an existing task. Example --add-tags work,test"));
			Handler = CommandHandler.Create<int, string, string>(UpdateTaskDetails);
			_repository = repository;
		}

		private void UpdateTaskDetails(int id, string title = null, string addTags = null)
		{
			Log.Debug($"Modifying information of task [{id}]. Params -> Title: {title}, Tags: {addTags}");

			var task = _repository.GetTaskById(id);
			if (null == task)
			{
				Log.Error($"Task with ID {id} does not exist.");
				Output.WriteError($"Task with ID {id} does not exist. View all tasks with -> dewit list");
				return;
			}

			// Modify the title of the task
			if (!string.IsNullOrEmpty(title))
				task.TaskDescription = title;

			// Add tag(s) to the task
			if (!string.IsNullOrEmpty(addTags))
			{
				addTags = Sanitizer.SanitizeTags(addTags);
				task.Tags = String.Join(',', task.Tags, addTags);
			}

			_repository.UpdateTask(task);
			var success = _repository.SaveChanges();

			if (success)
			{
				Log.Information($"Successfully updated task : {task.Id} | {task.TaskDescription} | {task.Tags}");
				Output.WriteText($"[green]Successfully updated task[/] : {task.Id} | {task.TaskDescription} | {task.Tags}");
			}
			else
			{
				Log.Error($"Failed to update task [{id}].");
				Output.WriteError($"Failed to update task details. Please try again.");
			}
		}
	}
}