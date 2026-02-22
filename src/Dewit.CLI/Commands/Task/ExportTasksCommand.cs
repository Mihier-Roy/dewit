using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Task
{
    public class ExportTasksCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly IDataConverter _dataConverter;
        private readonly IConfigurationService _configService;
        private readonly Option<FileInfo?> _pathOpt;
        private readonly Option<string> _formatOpt;

        public ExportTasksCommand(
            ITaskService taskService,
            IDataConverter dataConverter,
            IConfigurationService configService
        )
            : base("export", "Export all your tasks to a CSV or JSON file")
        {
            _taskService = taskService;
            _dataConverter = dataConverter;
            _configService = configService;

            _pathOpt = new Option<FileInfo?>("--path")
            {
                Description =
                    "Path to where the exported data is to be saved. By default, it will be saved in the current directory.",
            };
            _formatOpt = new Option<string>("--format")
            {
                Description =
                    "Data format in which the exported data is to be saved. Default format is JSON.",
                DefaultValueFactory = _ => "json",
            };
            _formatOpt.AcceptOnlyFromAmong("csv", "json");

            Options.Add(_pathOpt);
            Options.Add(_formatOpt);

            SetAction(parseResult =>
            {
                var path = parseResult.GetValue(_pathOpt);
                var format = parseResult.GetValue(_formatOpt)!;
                ExportTasks(path, format);
            });
        }

        private void ExportTasks(FileInfo? path, string format)
        {
            if (null == path)
            {
                path = new FileInfo(Directory.GetCurrentDirectory());
            }

            var title = _configService.GetValue($"export.{format}.title") ?? "dewit_tasks";
            string filePath = Path.Combine(path.ToString(), $"{title}.{format}");

            Output.WriteVerbose($"Exporting all tasks to file. Format: {format}, Path : {path}");
            var tasks = _taskService.GetTasks(duration: "all").OrderBy(p => p.AddedOn);

            try
            {
                var dataFormat = format.ToLower() == "csv" ? DataFormats.Csv : DataFormats.Json;
                _dataConverter.ExportToFile(tasks, filePath, dataFormat);
                Output.WriteText($"[green]Succesfully exported data.[/] Path : {filePath}");
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Failed to export data");
                Output.WriteError("Failed to export data to file. Please try again.");
            }
        }
    }
}
