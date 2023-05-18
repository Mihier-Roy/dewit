using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;
using Spectre.Console.Cli;

namespace Dewit.CLI.Branches.DataTransfer
{
	public class ImportTasksCommand : Command<ImportTasksCommand.Settings>
	{
		public class Settings : CommandSettings
		{
			[CommandArgument(0, "<file-path>")]
			[Description("Directory where the exported file should be stored.")]
			public string FilePath { get; set; }

			[CommandOption("-f|--format <data-format>")]
			[Description("Format of the import file: json/csv")]
			public DataFormats Format { get; set; }
		}
		
		private readonly IDataConverter<TaskItem> _dataConverterService;
		private readonly ITaskService _taskService;
		private readonly ILogger<ImportTasksCommand> _logger;

		public ImportTasksCommand(IDataConverter<TaskItem> dataConverterService, ITaskService taskService, ILogger<ImportTasksCommand> logger)
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

			_logger.LogDebug("Importing data from file. Format: {Format}, Path: {Path}", settings.Format.ToString(), path);
			if (!path.Exists)
			{
				_logger.LogError("Import file not found");
				Output.WriteError($"The entered path does not exist. Please try again.");
				return -1;
			}

			try
			{
				var tasksFromFile = _dataConverterService.FromFormat(settings.Format, path.ToString()).ToList();

				foreach (var task in tasksFromFile)
				{
					_taskService.ImportTask(task);
				}

				Output.WriteText($"[green]Succesfully imported data.[/] Path : {path.ToString()}");
				Output.WriteTasksTable(new string[] { "ID", "Task", "Status", "Tags", "Added On", "Completed On" }, tasksFromFile);
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Failed to import data.");
				Output.WriteError("Failed to import data from file. Please try again.");
				return -1;
			}
			
			return 0;
		}
	}
}