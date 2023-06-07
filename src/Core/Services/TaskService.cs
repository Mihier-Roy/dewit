using System.Globalization;
using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Dewit.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Dewit.Core.Services
{
	public class TaskService : ITaskService
	{
		private readonly IRepository<TaskItem> _taskRepository;
		private readonly ILogger<TaskService> _logger;

		public TaskService(IRepository<TaskItem> taskRepository, ILogger<TaskService> logger)
		{
			_taskRepository = taskRepository;
			_logger = logger;
		}

		public void AddTask(string title, string status, string? tags = null)
		{
			if (null != tags)
			{
				tags = Sanitizer.SanitizeTags(tags);
				tags = Sanitizer.DeduplicateTags(tags);
			}

			_logger.LogInformation("Adding a new task : {title}", title);
			var newTask = new TaskItem(title, status, tags, DateTime.Now);

			try
			{
				_taskRepository.Add(newTask);
				_logger.LogInformation($"Added a new task : {title}, Status = {status}, Tags = {tags}", title, status, tags);
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to add task. Exception stack : ", ex);
				throw new ApplicationException("Failed to add task");
			}
		}

		public void DeleteTask(int id)
		{
			_logger.LogInformation("Deleting task [{id}].", id);
			var task = _taskRepository.GetById(id);

			if (null == task)
			{
				_logger.LogError("Task with ID {id} does not exist.", id);
				throw new ArgumentException("Task not found");
			}

			try
			{
				_taskRepository.Remove(task);
				_logger.LogInformation("Deleted task : {Id} | {TaskDescription}", task.Id, task.TaskDescription);
			}
			catch(Exception e)
			{
				_logger.LogError("Failed to delete task [{id}]. {e}", id, e);
				throw new ApplicationException("Failed to delete task");
			}
		}

		public IEnumerable<TaskItem> GetTasks(string sort = "date", string duration = "today", string? status = null, string? tags = null, string? search = null)
		{
			_logger.LogInformation("Showing all tasks with arguments -> sort: {sort}, duration : {duration}, status: {status}, tags: {tags}, seach string : {search}", sort, duration, status, tags, search);
			var tasks = _taskRepository.List();
			List<TaskItem> tempList = new();

			tasks = duration switch
			{
				"yesterday" => tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-1)),
				"today" => tasks.Where(p => p.AddedOn.Date == DateTime.Today.Date),
				"week" => tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-7)),
				"month" => tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-30)),
				_ => tasks
			};

			tasks = status switch
			{
				"doing" => tasks.Where(p => p.Status == "Doing"),
				"done" => tasks.Where(p => p.Status == "Done"),
				"later" => tasks.Where(p => p.Status == "Later"),
				_ => tasks
			};

			if (null != search)
			{
				tasks = tasks.Where(p => p.TaskDescription.Contains(search));
			}

			// Filter tasks by tags
			if (null != tags)
			{
				tags = Sanitizer.SanitizeTags(tags);
				string[] allTags = tags.Contains(',') ? tags.Split(',') : new string[] { tags };

				foreach (string tag in allTags)
				{
					var test = tasks.Where(p => !string.IsNullOrEmpty(p.Tags) && p.Tags.Split(',').Contains(tag));
					tempList.AddRange(test);
				}

				// Assign final output
				tasks = tempList.Distinct();
			}

			if (sort == "status")
				tasks = tasks.OrderBy(p => p.Status);
			else
				tasks = tasks.OrderBy(p => p.AddedOn);

			return tasks.ToList();
		}

		public void UpdateStatus(int id, string completedAt)
		{
			_logger.LogInformation("Setting status of task [{id}] to Done", id);

			var task = _taskRepository.GetById(id);
			if (null == task)
			{
				_logger.LogError("Task with ID {id} does not exist.", id);
				return;
			}

			// If the completed-at option is provided, parse the date entered by the user
			if (!string.IsNullOrEmpty(completedAt))
			{
				var culture = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
				var styles = DateTimeStyles.AssumeLocal;

				if (DateTime.TryParse(completedAt, culture, styles, out DateTime completedOn))
					task.CompletedOn = completedOn;
				else
				{
					_logger.LogError("Failed to set status of task [{id}] to Done", id);
				}
			}
			else
				task.CompletedOn = DateTime.Now;

			task.Status = "Done";

			try
			{
				_taskRepository.Update(task);
				_logger.LogInformation("Completed task : {Id} | {TaskDescription}", task.Id, task.TaskDescription);
			}
			catch
			{
				_logger.LogError("Failed to set status of task [{id}] to Done", id);
			}
		}

		public void UpdateTaskDetails(int id, string? title = null, string? addTags = null, string? removeTags = null, bool resetTags = false)
		{
			_logger.LogInformation("Modifying information of task [{id}]. Params -> Title: {title}, Tags: {addTags}", id, title, addTags);

			var task = _taskRepository.GetById(id);
			if (null == task)
			{
				_logger.LogError("Task with ID {id} does not exist.", id);
				return;
			}

			// Modify the title of the task
			if (!string.IsNullOrEmpty(title))
				task.TaskDescription = title;

			// Add tag(s) to the task
			if (!string.IsNullOrEmpty(addTags))
			{
				addTags = Sanitizer.SanitizeTags(addTags);
				var updatedTags = string.Join(',', task.Tags, addTags);
				updatedTags = Sanitizer.DeduplicateTags(updatedTags);
				task.Tags = updatedTags[0] == ',' ? updatedTags.Remove(0, 1) : updatedTags;
			}

			// Remove tag(s) from a task
			if (!string.IsNullOrEmpty(removeTags))
			{
				var tagsToRemove = Sanitizer.SanitizeTags(removeTags).Split(',');
				var oldTags = task.Tags.Split(',');
				task.Tags = string.Join(',', oldTags.Except(tagsToRemove));
			}

			// Remove all tags from a task
			if (resetTags)
			{
				task.Tags = string.Empty;
			}

			try
			{
				_taskRepository.Update(task);
				_logger.LogInformation("Successfully updated task : {Id} | {TaskDescription} | {Tags}", task.Id, task.TaskDescription, task.Tags);
			}
			catch
			{
				_logger.LogError("Failed to update task [{id}].", id);
			}
		}

		public void ImportTask(TaskItem taskItem)
		{
			try
			{
				_taskRepository.Add(taskItem);
				_logger.LogInformation("Added a new task : {TaskDescription}, Status = {Status}, Tags = {Tags}", taskItem.TaskDescription, taskItem.Status, taskItem.Tags);
			}
			catch (Exception ex)
			{
				_logger.LogError("Failed to add task. Exception stack : ", ex);
			}
		}
	}
}