using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Dewit.Core.Utils;

namespace Dewit.Core.Services
{
	public class TaskService : ITaskService
	{
		private readonly IRepository<TaskItem> _taskRepository;

		public TaskService(IRepository<TaskItem> taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public void AddTask(string title, string status, string? tags = null)
		{
			if (null != tags)
			{
				tags = Sanitizer.SanitizeTags(tags);
				tags = Sanitizer.DeduplicateTags(tags);
			}

			// Log.Debug($"Adding a new task : {title}, Status = {(_name == "now" ? "Doing" : "Later")}, Tags = {tags}");
			var newTask = new TaskItem(title, status, tags, DateTime.Now);

			try
			{
				_taskRepository.Add(newTask);
				// Log.Information($"Added a new task : {title}, Status = {(_name == "now" ? "Doing" : "Later")}, Tags = {tags}");
				// Output.WriteText($"[green]Added a new task[/] : {title}, [aqua]Status[/] = {(_name == "now" ? "Doing" : "Later")}, [aqua]Tags[/] = {tags}");
			}
			catch
			{
				// Log.Error($"Failed to add task.");
				// Output.WriteError($"Failed to add task. Please try again.");
			}
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