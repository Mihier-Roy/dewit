namespace Dewit.Core.Interfaces
{
	interface ITaskService
	{
		void GetTasks(string sort = "date", string duration = "today", string? status = null, string? tags = null, string? search = null);
		void AddTask(string title, string status, string? tags = null);
		void DeleteTask(int id);
		void UpdateStatus(int id, string completedAt);
		void UpdateTaskDetails(int id, string? title = null, string? addTags = null, string? removeTags = null, bool resetTags = false);
	}
}