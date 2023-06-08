using System;
using System.ComponentModel;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Spectre.Console.Cli;

namespace Dewit.CLI.Branches.Task
{
	public class AddTaskCommand : Command<AddTaskCommand.Settings>
	{
		public class Settings : CommandSettings
		{
			[CommandArgument(0, "<title>")]
			[Description("Description of the task you're performing")]
			public string Title { get; set; }

			[CommandOption("-t|--tags <tags>")]
			[Description("Comma separated list of tags")]
			public string Tags { get; set; }
		}
		
		private readonly ITaskService _taskService;

		public AddTaskCommand(ITaskService taskService)
		{
			
			_taskService = taskService;
		}
 
		public override int Execute(CommandContext context, Settings settings)
		{
			try
			{
				var status = context.Name == "now" ? "Doing" : "Later";
				_taskService.AddTask(settings.Title, status, settings.Tags);
				Output.WriteText($"[green]Added a new task[/] : {settings.Title}, [aqua]Status[/] = {(context.Name == "now" ? "Doing" : "Later")}, [aqua]Tags[/] = {settings.Tags}");
			}
			catch (ApplicationException e)
			{
				Output.WriteError($"Failed to add task. Please try again.");
				return -1;
			}

			return 0;
		}
	}
}