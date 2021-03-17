using System.CommandLine;
using Dewit.CLI.Commands;
using Dewit.CLI.Data;

namespace Dewit.CLI
{
	public class App
	{
		private ITaskRepository _repository;

		public App(ITaskRepository repository)
		{
			_repository = repository;
		}

		public void Run(string[] args)
		{
			var rootCommand = new RootCommand("dewit"){
				new AddTaskCommand(_repository, "now"),
				new AddTaskCommand(_repository, "later"),
				new UpdateTaskCommand(_repository, "done"),
				new GetTasksCommand(_repository, "list"),
			};

			rootCommand.InvokeAsync(args).Wait();
		}
	}
}