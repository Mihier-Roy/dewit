using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Dewit.CLI.Data;
using Dewit.CLI.Utils;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class DeleteTaskCommand : Command
    {
        private readonly ITaskRepository _repository;

        public DeleteTaskCommand(ITaskRepository repository, string name, string description = null) : base(name, description)
        {
            AddArgument(new Argument<int>("id", "ID of the task to be deleted."));
            Handler = CommandHandler.Create<int>(DeleteTask);
            _repository = repository;
        }

        private void DeleteTask(int id)
        {
            Log.Debug($"Deleting task [{id}].");
            var task = _repository.GetTaskById(id);

            if (null == task)
            {
                Log.Error($"Task with ID {id} does not exist.");
                Output.WriteError($"Task with ID {id} does not exist. View all tasks with -> dewit list");
                return;
            }

            _repository.RemoveTask(task);
            var success = _repository.SaveChanges();

            if (success)
            {
                Log.Information($"Deleted task : {task.Id} | {task.TaskDescription} ");
                Output.WriteText($"[yellow]Deleted task[/] : {task.Id} | {task.TaskDescription} ");
            }
            else
            {
                Log.Error($"Failed to delete task [{id}].");
                Output.WriteError($"Failed to delete. Please try again.");
            }
        }
    }
}
