using System.CommandLine;
using Dewit.CLI.Commands;

namespace Dewit.CLI
{
	public class App
	{
		public void Run(string[] args)
		{
			var rootCommand = new RootCommand("dewit"){
				new AddTaskCommand("now"),
				new AddTaskCommand("later"),
				new UpdateStatusCommand("done"),
				new UpdateTaskCommand("edit"),
				new GetTasksCommand("list"),
				new DeleteTaskCommand("delete"),
				// new ExportTasksCommand("export"),
				// new ImportTasksCommand("import"),
			};

			rootCommand.InvokeAsync(args).Wait();
		}
	}
}