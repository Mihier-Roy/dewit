using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Dewit.CLI.Branches.DataTransfer
{
	public class ExportTasksCommand : Command<ExportTasksCommand.Settings>
	{
		public class Settings : CommandSettings
		{
			[CommandArgument(0, "<file-path>")]
			[Description("Directory where the exported file should be stored.")]
			public string FilePath { get; set; }

			[CommandOption("-f|--format <data-format>")]
			[Description("New student enrollment date.")]
			public DataFormats Format { get; set; }
		}
		
		private readonly IDataConverter<TaskItem> _dataConverterService;
		private readonly ITaskService _taskService;
		private readonly ILogger<ExportTasksCommand> _logger;

		public ExportTasksCommand(IDataConverter<TaskItem> dataConverterService, ITaskService taskService, ILogger<ExportTasksCommand> logger)
		{

			_dataConverterService = dataConverterService;
			_taskService = taskService;
			_logger = logger;
		}
 
		public override int Execute(CommandContext context, Settings settings)
		{
			FileSystemInfo path = null;
			if (null == settings.FilePath)
			{
				path = new FileInfo(Directory.GetCurrentDirectory());
			}
			
			try
			{
				_logger.LogDebug("Exporting all tasks to file. Format: {Format}, Path : {Path}", settings.Format, path.ToString());
				
				var tasks = _taskService.GetTasks().OrderBy(task => task.AddedOn);
				_dataConverterService.ToFormat(settings.Format, path.ToString(), tasks);
				
				_logger.LogInformation("Exported data to Path : {Path}", settings.FilePath);
				Output.WriteText($"[green]Successfully exported data.[/] Path : {settings.FilePath}");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Failed to export data.");
				Output.WriteError("Failed to export data to file. Please try again.");
			}
			return 0;
		}
	}
}