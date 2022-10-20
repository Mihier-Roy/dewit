using Dewit.Core.Interfaces;

namespace Dewit.Core.Services
{
	public class TaskService : ITaskService
	{
		public void AddTask(string title, string? tags = null)
		{
			throw new NotImplementedException();
		}

		public void DeleteTask(int id)
		{
			throw new NotImplementedException();
		}

		public void GetTasks(string sort = "date", string duration = "today", string? status = null, string? tags = null, string? search = null)
		{
			throw new NotImplementedException();
		}

		public void UpdateStatus(int id, string completedAt)
		{
			throw new NotImplementedException();
		}

		public void UpdateTaskDetails(int id, string? title = null, string? addTags = null, string? removeTags = null, bool resetTags = false)
		{
			throw new NotImplementedException();
		}
	}
}