using System.Collections.Generic;
using Dewit.CLI.Models;

namespace Dewit.CLI.Data
{
	public class SqlTaskRepository : ITaskRepository
	{
		public void AddTask(TaskItem task)
		{
			throw new System.NotImplementedException();
		}

		public TaskItem GetTaskById(int id)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<TaskItem> GetTasks()
		{
			throw new System.NotImplementedException();
		}

		public bool SaveChanges()
		{
			throw new System.NotImplementedException();
		}

		public void UpdateTask(TaskItem task)
		{
			throw new System.NotImplementedException();
		}
	}
}