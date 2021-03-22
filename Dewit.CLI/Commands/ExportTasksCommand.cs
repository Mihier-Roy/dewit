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
			AddOption(new Option<FileInfo>("--file-path", "Path to where the exported data is to be saved. By default, it will be saved in the current directory."));
			Handler = CommandHandler.Create<FileSystemInfo>(ExportTasks);
			_repository = repository;
		}

		private void ExportTasks(FileSystemInfo exportPath = null)
		{
			if (null == exportPath)
			{
				exportPath = new FileInfo(Directory.GetCurrentDirectory());
			}

			string filePath = Path.Combine(exportPath.ToString(), "dewit_tasks.csv");

			Log.Debug($"Exporting all tasks to file. Path : {exportPath}");
			var tasks = _repository.GetTasks().OrderBy(p => p.AddedOn);

			FormatData.ToCsv(tasks, filePath);
			Output.WriteText($"[green]Successfully exported tasks[/]. Data exported to : {filePath}");
		}
	}
}