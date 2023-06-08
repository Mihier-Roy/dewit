using System;
using System.ComponentModel;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Spectre.Console.Cli;

namespace Dewit.CLI.Branches.Task
{
	public class CompleteTaskCommand : Command<CompleteTaskCommand.Settings>
	{
		public class Settings : CommandSettings
		{
			[CommandArgument(0, "<title>")]
			[Description("ID of the task to be updated")]
			public int Id { get; set; }
			
			[CommandOption("-c|--completed-at [time]")]
			[Description("Specify when the task was completed")]
			public string CompletedAt { get; set; }
		}
		
		private readonly ITaskService _taskService;

		public CompleteTaskCommand(ITaskService taskService)
		{
			
			_taskService = taskService;
		}
 
		public override int Execute(CommandContext context, Settings settings)
		{
			try
			{
				_taskService.CompleteTask(settings.Id, settings.CompletedAt);
				Output.WriteText($"[green]Completed task[/] : {settings.Id} ");
			}
			catch (ArgumentException e)
			{
				Output.WriteError($"Task with ID {settings.Id} does not exist. View all tasks with -> dewit list");
				return -1;
			}
			catch (ApplicationException e)
			{
				Output.WriteError("Failed to set task as completed. Please try again.");
				return -1;
			}

			return 0;
		}
	}
}