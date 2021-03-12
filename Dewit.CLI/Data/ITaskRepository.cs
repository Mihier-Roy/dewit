using System.Collections.Generic;
using Dewit.CLI.Models;

namespace Dewit.CLI.Data
{
	public interface ITaskRepository
	{
		IEnumerable<TaskItem> GetTasks();
		void AddTask(TaskItem task);
	}
}