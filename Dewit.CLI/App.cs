using System;
using System.Collections.Generic;
using Dewit.CLI.Data;
using Dewit.CLI.Models;
using Microsoft.Extensions.Configuration;

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

			if (null != args[0] && acceptedArgs.Contains(args[0]))
			{
				if (args[0] == "now" || args[0] == "later")
				{
					Console.WriteLine($"Adding task : {args[1]}");
					var newTask = new TaskItem { Id = 1, TaskDescription = args[1], AddedOn = DateTime.Now };
					_repository.AddTask(newTask);
				}

				if (args[0] == "done")
				{
					Console.WriteLine($"Completed task : {args[1]}");
				}

				if (args[0] == "list")
				{
					Console.WriteLine("Showing all tasks");
					var tasks = _repository.GetTasks();
					foreach (var task in tasks)
					{
						Console.WriteLine($"[{task.AddedOn}] {task.TaskDescription}");
					}
				}
			}
		}
	}
}