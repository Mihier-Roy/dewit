using System;
using System.ComponentModel;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Spectre.Console.Cli;

namespace Dewit.CLI.Branches.Task
{
	public class UpdateTaskCommand : Command<UpdateTaskCommand.Settings>
	{
		public class Settings : CommandSettings
		{
			[CommandArgument(0, "<id>")]
			[Description("ID of the task to be updated")]
			public int Id { get; set; }
			
			[CommandOption("--title [title]")]
			[Description("Change the description of the task")]
			public string? Title { get; set; }
			
			[CommandOption("--add-tags [tags]")]
			[Description("Add new tag(s) to an existing task. Example --add-tags work,test")]
			public string? AddTags { get; set; }
			
			[CommandOption("--remove-tags [tags]")]
			[Description("Remove tag(s) from an existing task. Example --remove-tags work,test")]
			public string? RemoveTags { get; set; }
			
			[CommandOption("--reset-tags [tags]")]
			[Description("Remove all tags from an existing task")]
			public bool ResetTags { get; set; }
		}
		
		private readonly ITaskService _taskService;

		public UpdateTaskCommand(ITaskService taskService)
		{
			_taskService = taskService;
		}
 
		public override int Execute(CommandContext context, Settings settings)
		{
			try
			{
				var task = _taskService.UpdateTaskDetails(settings.Id, settings.Title, settings.AddTags, settings.RemoveTags, settings.ResetTags);
				Output.WriteText($"[green]Successfully updated task[/] : {task.Id} | {task.TaskDescription} | {task.Tags}");
			}
			catch (ArgumentException)
			{
				Output.WriteError($"Task with ID {settings.Id} does not exist. View all tasks with -> dewit list");
				return -1;
			}
			catch (ApplicationException)
            {
				Output.WriteError($"Failed to update task details. Please try again.");
				return -1;
			}

			return 0;
		}
	}
}