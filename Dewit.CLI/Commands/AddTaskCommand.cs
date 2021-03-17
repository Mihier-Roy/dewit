using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Dewit.CLI.Data;
using Dewit.CLI.Models;
using Dewit.CLI.Utils;
using Serilog;

namespace Dewit.CLI.Commands
{
	public class AddTaskCommand : Command
	{
		private readonly ITaskRepository _repository;
		private readonly string _name;

		public AddTaskCommand(ITaskRepository repository, string name, string description = null) : base(name, description)
		{
			AddArgument(new Argument<string>("title", "Description of the task you're currently performing."));
			Handler = CommandHandler.Create<string>(AddTask);
			_repository = repository;
			_name = name;
		}

		private void AddTask(string title)
		{
			Log.Debug($"Adding a new task : {title}, Status = {(_name == "now" ? "Doing" : "Later")}");
			var newTask = new TaskItem
			{
				TaskDescription = title,
				AddedOn = DateTime.Now,
				Status = _name == "now" ? "Doing" : "Later"
			};

			_repository.AddTask(newTask);
			var success = _repository.SaveChanges();

			if (success)
			{
				Log.Information($"Added a new task : {title}, Status = {(_name == "now" ? "Doing" : "Later")}");
				Output.WriteText($"[green]Added a new task[/] : {title}, Status = {(_name == "now" ? "Doing" : "Later")}");
			}
			else
			{
				Log.Error($"Failed to add task.");
				Output.WriteError($"Failed to add task. Please try again.");
			}
		}
	}
}