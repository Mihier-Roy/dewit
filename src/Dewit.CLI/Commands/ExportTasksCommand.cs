using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using Dewit.CLI.Models;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class ExportTasksCommand : Command
    {
        private readonly ITaskService _taskService;

        public ExportTasksCommand(ITaskService taskService, string name, string? description = null) : base(name, description)
        {
            var filePathOption = new Option<FileInfo>("--path", "Path to where the exported data is to be saved. By default, it will be saved in the current directory.");
            var formatOption = new Option<string>("--format", "Data format in which the exported data is to be saved. Default format is JSON.")
                                    .FromAmong("csv", "json");
            AddOption(filePathOption);
            AddOption(formatOption);
            Handler = CommandHandler.Create<FileSystemInfo, string>(ExportTasks);
            _taskService = taskService;
        }

        private void ExportTasks(FileSystemInfo? path = null, string format = "json")
        {

            if (null == path)
            {
                path = new FileInfo(Directory.GetCurrentDirectory());
            }

            string filePath = Path.Combine(path.ToString(), $"dewit_tasks.{format}");

            Log.Debug($"Exporting all tasks to file. Format: {format}, Path : {path}");
            var tasks = _taskService.GetTasks(duration: "all").OrderBy(p => p.AddedOn);

            // Convert to old TaskItem format for export compatibility
            var oldFormatTasks = tasks.Select(t => new TaskItem
            {
                Id = t.Id,
                TaskDescription = t.TaskDescription,
                Status = t.Status,
                Tags = t.Tags,
                AddedOn = t.AddedOn,
                CompletedOn = t.CompletedOn
            });

            try
            {
                FormatData.ToType(oldFormatTasks, filePath, format);
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
