using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Dewit.CLI.Data;
using Dewit.CLI.Utils;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class ImportTasksCommand : Command
    {
        private readonly ITaskRepository _repository;

        public ImportTasksCommand(ITaskRepository repository, string name, string? description = null) : base(name, description)
        {
            AddArgument(new Argument<FileInfo>("path", "Path to import data."));
            AddOption(new Option<string>("--format", "Import format. Default format is JSON.").FromAmong("csv", "json"));
            Handler = CommandHandler.Create<FileInfo, string>(ImportTasks);
            _repository = repository;
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
                foreach (var task in tasksFromFile)
                {
                    task.Id = null;
                    _repository.AddTask(task);
                }
                _repository.SaveChanges();

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
