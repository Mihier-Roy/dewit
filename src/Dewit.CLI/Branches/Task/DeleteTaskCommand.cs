using System;
using System.ComponentModel;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Dewit.CLI.Branches.Task
{
	public class DeleteTaskCommand : Command<DeleteTaskCommand.Settings>
	{
		public class Settings : CommandSettings
		{
			[CommandArgument(0, "<title>")]
			[Description("ID of the task to be deleted")]
			public int Id { get; set; }
		}
		
		private readonly ITaskService _taskService;
		private readonly ILogger<Task.DeleteTaskCommand> _logger;

		public DeleteTaskCommand(ITaskService taskService, ILogger<Task.DeleteTaskCommand> logger)
		{
			
			_taskService = taskService;
			_logger = logger;
		}
 
		public override int Execute(CommandContext context, Settings settings)
		{
			try
			{
				_taskService.DeleteTask(settings.Id);
				Output.WriteText($"[yellow]Deleted task[/] : {settings.Id}");
			}
			catch (ArgumentException e)
			{
				Output.WriteError($"Task with ID {settings.Id} does not exist. View all tasks with -> dewit list");
				return -1;
			}
			catch (ApplicationException e)
			{
				Output.WriteError($"Failed to delete task. Please try again.");
				return -1;
			}

			return 0;
		}
	}
}