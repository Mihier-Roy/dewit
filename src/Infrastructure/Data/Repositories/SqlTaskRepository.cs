using System;
using System.Collections.Generic;
using System.Linq;
using Dewit.CLI.Models;

namespace Dewit.CLI.Data
{
	public class SqlTaskRepository : ITaskRepository
	{
		private readonly TaskContext _context;

		public SqlTaskRepository(TaskContext context)
		{
			_context = context;
		}
		public void AddTask(TaskItem task)
		{
			if (null == task)
			{
				throw new ArgumentNullException(nameof(task));
			}
			_context.Tasks.Add(task);
		}

		public TaskItem GetTaskById(int id)
		{
			return _context.Tasks.FirstOrDefault(t => t.Id == id);
		}

		public IEnumerable<TaskItem> GetTasks()
		{
			return _context.Tasks.ToList();
		}

		public void RemoveTask(TaskItem task)
		{
			if (null == task)
			{
				throw new ArgumentNullException(nameof(task));
			}
			_context.Tasks.Remove(task);
		}

		public bool SaveChanges()
		{
			return _context.SaveChanges() >= 0;
		}

		public void UpdateTask(TaskItem task)
		{
			// No implementation required because EF will track changes to objects.
		}
	}
}