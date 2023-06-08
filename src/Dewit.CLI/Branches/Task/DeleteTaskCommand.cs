using System;
using System.ComponentModel;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Spectre.Console.Cli;

namespace Dewit.CLI.Branches.Task
{
	public class DeleteTaskCommand : Command<DeleteTaskCommand.Settings>
	{
		public class Settings : CommandSettings
		{
			[CommandArgument(0, "<id>")]
			[Description("ID of the task to be deleted")]
			public int Id { get; set; }
		}
		
		private readonly ITaskService _taskService;

		public DeleteTaskCommand(ITaskService taskService)
		{
			_taskService = taskService;
		}
 
		public override int Execute(CommandContext context, Settings settings)
		{
			try
			{
				_taskService.DeleteTask(settings.Id);
				Output.WriteText($"[yellow]Deleted task[/] : {settings.Id}");
			}
			catch (ArgumentException)
			{
				Output.WriteError($"Task with ID {settings.Id} does not exist. View all tasks with -> dewit list");
				return -1;
			}
			catch (ApplicationException)
			{
				Output.WriteError($"Failed to delete task. Please try again.");
				return -1;
			}

			return 0;
		}
	}
}