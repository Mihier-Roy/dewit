using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using Dewit.CLI.Data;
using Dewit.CLI.Utils;
using Serilog;

namespace Dewit.CLI.Commands
{
	public class ExportTasksCommand : Command
	{
		private readonly ITaskRepository _repository;

		public ExportTasksCommand(ITaskRepository repository, string name, string description = null) : base(name, description)
		{
			var filePathOption = new Option<FileInfo>("--path", "Path to where the exported data is to be saved. By default, it will be saved in the current directory.");
			var formatOption = new Option<string>("--format", "Data format in which the exported data is to be saved. Default format is JSON.")
									.FromAmong("csv", "json");
			AddOption(filePathOption);
			AddOption(formatOption);
			Handler = CommandHandler.Create<FileSystemInfo, string>(ExportTasks);
			_repository = repository;
		}

		private void ExportTasks(FileSystemInfo path = null, string format = "json")
		{

			if (null == path)
			{
				path = new FileInfo(Directory.GetCurrentDirectory());
			}

			string filePath = Path.Combine(path.ToString(), $"dewit_tasks.{format}");

			Log.Debug($"Exporting all tasks to file. Format: {format}, Path : {path}");
			var tasks = _repository.GetTasks().OrderBy(p => p.AddedOn);

			try
			{
				FormatData.ToType(tasks, filePath, format);
				Output.WriteText($"[green]Succesfully exported data.[/] Path : {filePath}");
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Failed to export data.");
				Output.WriteError("Failed to export data to file. Please try again.");
			}

		}
	}
}