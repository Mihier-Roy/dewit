using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Dewit.CLI.Utils;
using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class ImportTasksCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly IDataConverter _dataConverter;

        public ImportTasksCommand(ITaskService taskService, IDataConverter dataConverter, string name, string? description = null) : base(name, description)
        {
            AddArgument(new Argument<FileInfo>("path", "Path to import data."));
            AddOption(new Option<string>("--format", "Import format. Default format is JSON.").FromAmong("csv", "json"));
            Handler = CommandHandler.Create<FileInfo, string>(ImportTasks);
            _taskService = taskService;
            _dataConverter = dataConverter;
        }

        private void ImportTasks(FileInfo path, string format = "json")
        {
            Log.Debug($"Importing data from file. Format: {format}, Path: {path}");
            if (!path.Exists)
            {
                Log.Error("File to import does not exist.");
                Output.WriteError($"The entered path does not exist. Please try again.");
                return;
            }

            try
            {
                var dataFormat = format.ToLower() == "csv" ? DataFormats.Csv : DataFormats.Json;
                var tasksFromFile = _dataConverter.ImportFromFile<TaskItem>(path.ToString(), dataFormat);

                foreach (var task in tasksFromFile)
                {
                    _taskService.ImportTask(task);
                }

                Output.WriteText($"[green]Succesfully imported data.[/] Path : {path}");
                Output.WriteTable(new string[] { "ID", "Task", "Status", "Tags", "Added On", "Completed On" }, tasksFromFile);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to import data.");
                Output.WriteError("Failed to import data from file. Please try again.");
            }
        }
    }
}
