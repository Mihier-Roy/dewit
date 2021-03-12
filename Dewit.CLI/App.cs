using System;
using System.Collections.Generic;
using Dewit.CLI.Data;
using Dewit.CLI.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Dewit.CLI
{
	public class App
	{
		private readonly IConfiguration _config;
		private readonly ITaskRepository _repository;

		public App(IConfiguration config, ITaskRepository repository)
		{
			_config = config;
			_repository = repository;
		}

		public void Run(string[] args)
		{
			List<string> acceptedArgs = new List<string>() { "now", "later", "done", "list" };

			if (args.Length > 0 && null != args[0] && acceptedArgs.Contains(args[0]))
			{
				switch (args[0])
				{
					case "now":
					case "later":
						{
							Log.Debug($"Adding a new task : {args[1]}");
							var newTask = new TaskItem { Id = 1, TaskDescription = args[1], AddedOn = DateTime.Now };
							_repository.AddTask(newTask);
							Console.WriteLine($"Added task : {args[1]}");
						}
						break;
					case "done":
						{
							Log.Debug($"Setting status of task [{args[1]}] to Completed");
							Console.WriteLine($"Completed task : {args[1]}");
						}
						break;
					case "list":
						{
							Log.Debug("Listing all tasks");
							Console.WriteLine("Showing all tasks");
							var tasks = _repository.GetTasks();
							foreach (var task in tasks)
							{
								Console.WriteLine($"[{task.AddedOn}] {task.TaskDescription}");
							}
						}
						break;
					default:
						Console.WriteLine("Invalid command.");
						break;
				}
			}
			else
			{
				Log.Error("Invalid arguments passed. Exiting.");
				Console.WriteLine("Invalid usage.");
			}
		}
	}
}