using System.CommandLine;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Task;

public class TaskCommand : Command
{
    public TaskCommand(
        ITaskService taskService,
        IDataConverter dataConverter,
        IConfigurationService configService
    )
        : base("task", "Manage all of your tasks")
    {
        Subcommands.Add(
            new AddTaskCommand(taskService, "now", "Create a new task and set it to 'Doing'")
        );
        Subcommands.Add(
            new AddTaskCommand(taskService, "later", "Create a new task and set it to 'Later'")
        );
        Subcommands.Add(new UpdateStatusCommand(taskService));
        Subcommands.Add(new UpdateTaskCommand(taskService));
        Subcommands.Add(new GetTasksCommand(taskService));
        Subcommands.Add(new DeleteTaskCommand(taskService));
        Subcommands.Add(new ExportTasksCommand(taskService, dataConverter, configService));
        Subcommands.Add(new ImportTasksCommand(taskService, dataConverter));
    }
}