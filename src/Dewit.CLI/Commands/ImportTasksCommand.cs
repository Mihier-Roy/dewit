using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Dewit.CLI.Utils;
using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class ImportTasksCommand : Command
    {
        private readonly ITaskService _taskService;

        public ImportTasksCommand(ITaskService taskService, string name, string? description = null) : base(name, description)
        {
            AddArgument(new Argument<FileInfo>("path", "Path to import data."));
            AddOption(new Option<string>("--format", "Import format. Default format is JSON.").FromAmong("csv", "json"));
            Handler = CommandHandler.Create<FileInfo, string>(ImportTasks);
            _taskService = taskService;
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
                var tasksFromFile = FormatData.FromType(path.ToString(), format);

                // Convert from old format to new and import
                foreach (var oldTask in tasksFromFile)
                {
                    var newTask = new TaskItem
                    {
                        TaskDescription = oldTask.TaskDescription,
                        Status = oldTask.Status,
                        Tags = oldTask.Tags ?? string.Empty,
                        AddedOn = oldTask.AddedOn,
                        CompletedOn = oldTask.CompletedOn
                    };
                    _taskService.ImportTask(newTask);
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
