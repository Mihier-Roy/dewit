using System.CommandLine;
using Dewit.CLI.Commands;
using Dewit.CLI.Data;

namespace Dewit.CLI
{
	public class App
	{
		private readonly ITaskRepository _repository;

		public App(ITaskRepository repository)
		{
			_repository = repository;
		}

		public void Run(string[] args)
		{
			var rootCommand = new RootCommand("dewit"){
				new AddTaskCommand(_repository, "now"),
				new AddTaskCommand(_repository, "later"),
				new UpdateStatusCommand(_repository, "done"),
				new UpdateTaskCommand(_repository, "edit"),
				new GetTasksCommand(_repository, "list"),
				new DeleteTaskCommand(_repository, "delete"),
				new ExportTasksCommand(_repository, "export"),
				new ImportTasksCommand(_repository, "import"),
			};

			rootCommand.InvokeAsync(args).Wait();
		}
	}
}