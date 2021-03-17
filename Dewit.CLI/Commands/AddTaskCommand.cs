using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.RegularExpressions;
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
			AddOption(new Option<string>("--tags", "Comma-separated list of tags to identify your task. Example : --tags work,testing"));
			Handler = CommandHandler.Create<string, string>(AddTask);
			_repository = repository;
			_name = name;
		}

		private void AddTask(string title, string tags = null)
		{
			if (null != tags)
			{
				Regex r = new Regex("(?:[^a-z0-9,_])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
				tags = r.Replace(tags, string.Empty);
				tags = tags[tags.Length - 1] == ',' ? tags.Remove(tags.Length - 1) : tags;
			}

			Log.Debug($"Adding a new task : {title}, Status = {(_name == "now" ? "Doing" : "Later")}, Tags = {tags}");
			var newTask = new TaskItem
			{
				TaskDescription = title,
				AddedOn = DateTime.Now,
				Status = _name == "now" ? "Doing" : "Later",
				Tags = tags
			};

			_repository.AddTask(newTask);
			var success = _repository.SaveChanges();

			if (success)
			{
				Log.Information($"Added a new task : {title}, Status = {(_name == "now" ? "Doing" : "Later")}, Tags = {tags}");
				Output.WriteText($"[green]Added a new task[/] : {title}, [aqua]Status[/] = {(_name == "now" ? "Doing" : "Later")}, [aqua]Tags[/] = {tags}");
			}
			else
			{
				Log.Error($"Failed to add task.");
				Output.WriteError($"Failed to add task. Please try again.");
			}
		}
	}
}