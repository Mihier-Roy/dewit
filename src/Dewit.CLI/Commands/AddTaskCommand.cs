using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class AddTaskCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly string _name;

        public AddTaskCommand(ITaskService taskService, string name, string? description = null) : base(name, description)
        {
            AddArgument(new Argument<string>("title", "Description of the task you're currently performing."));
            AddOption(new Option<string>("--tags", "Comma-separated list of tags to identify your task. Example : --tags work,testing"));
            Handler = CommandHandler.Create<string, string>(AddTask);
            _taskService = taskService;
            _name = name;
        }

        private void AddTask(string title, string? tags = null)
        {
            try
            {
                var status = _name == "now" ? "Doing" : "Later";
                _taskService.AddTask(title, status, tags);

                Log.Information($"Added a new task : {title}, Status = {status}, Tags = {tags}");
                Output.WriteText($"[green]Added a new task[/] : {title}, [aqua]Status[/] = {status}, [aqua]Tags[/] = {tags}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add task");
                Output.WriteError($"Failed to add task. Please try again.");
            }
        }
    }
}
