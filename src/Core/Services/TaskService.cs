using System.Globalization;
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
			// Log.Debug($"Deleting task [{id}].");
			var task = _taskRepository.GetById(id);

			if (null == task)
			{
				// Log.Error($"Task with ID {id} does not exist.");
				// Output.WriteError($"Task with ID {id} does not exist. View all tasks with -> dewit list");
				return;
			}

			try
			{
				_taskRepository.Remove(task);
				// Log.Information($"Deleted task : {task.Id} | {task.TaskDescription} ");
				// Output.WriteText($"[yellow]Deleted task[/] : {task.Id} | {task.TaskDescription} ");
			}
			catch
			{
				// Log.Error($"Failed to delete task [{id}].");
				// Output.WriteError($"Failed to delete. Please try again.");
			}
		}

		public void GetTasks(string sort = "date", string duration = "today", string? status = null, string? tags = null, string? search = null)
		{
			// Log.Debug($"Showing all tasks with arguments -> sort: {sort}, duration : {duration}, status: {status}, tags: {tags}, seach string : {search}");
			var tasks = _taskRepository.List();
			List<TaskItem> tempList = new();

			switch (duration)
			{
				case "yesterday":
					tasks = tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-1));
					break;
				case "today":
					tasks = tasks.Where(p => p.AddedOn.Date == DateTime.Today.Date);
					break;
				case "week":
					tasks = tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-7));
					break;
				case "month":
					tasks = tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-30));
					break;
				case "all": break;
			}

			switch (status)
			{
				case "doing":
					tasks = tasks.Where(p => p.Status == "Doing");
					break;
				case "done":
					tasks = tasks.Where(p => p.Status == "Done");
					break;
				case "later":
					tasks = tasks.Where(p => p.Status == "Later");
					break;
			}

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
					var test = tasks.Where(p => (!string.IsNullOrEmpty(p.Tags) && p.Tags.Split(',').Contains<string>(tag)));
					tempList.AddRange(test);
				}

				// Assign final output
				tasks = tempList.Distinct();
			}

			if (sort == "status")
				tasks = tasks.OrderBy(p => p.Status);
			else
				tasks = tasks.OrderBy(p => p.AddedOn);

			// Output.WriteText($"Displaying tasks using parameters -> [aqua]sort[/]: {sort}, [aqua]duration[/] : {duration}, [aqua]status[/]: {status ?? "n/a"}, [aqua]tags[/]:{tags}");
			// Output.WriteTable(new string[] { "ID", "Task", "Status", "Tags", "Added On", "Completed On" }, tasks);
		}

		public void UpdateStatus(int id, string completedAt)
		{
			// Log.Debug($"Setting status of task [{id}] to Done");

			var task = _taskRepository.GetById(id);
			if (null == task)
			{
				// Log.Error($"Task with ID {id} does not exist.");
				// Output.WriteError($"Task with ID {id} does not exist. View all tasks with -> dewit list");
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
					// Log.Error($"Failed to set status of task [{id}] to Done");
					// Output.WriteError("Failed to set task as completed. Please try again.");
				}
			}
			else
				task.CompletedOn = DateTime.Now;

			task.Status = "Done";

			try
			{
				_taskRepository.Update(task);
				// Log.Information($"Completed task : {task.Id} | {task.TaskDescription} ");
				// Output.WriteText($"[green]Completed task[/] : {task.Id} | {task.TaskDescription} ");
			}
			catch
			{
				// Log.Error($"Failed to set status of task [{id}] to Done");
				// Output.WriteError($"Failed to set task as completed. Please try again.");
			}
		}

		public void UpdateTaskDetails(int id, string? title = null, string? addTags = null, string? removeTags = null, bool resetTags = false)
		{
			// Log.Debug($"Modifying information of task [{id}]. Params -> Title: {title}, Tags: {addTags}");

			var task = _taskRepository.GetById(id);
			if (null == task)
			{
				// Log.Error($"Task with ID {id} does not exist.");
				// Output.WriteError($"Task with ID {id} does not exist. View all tasks with -> dewit list");
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
				// Log.Information($"Successfully updated task : {task.Id} | {task.TaskDescription} | {task.Tags}");
				// Output.WriteText($"[green]Successfully updated task[/] : {task.Id} | {task.TaskDescription} | {task.Tags}");
			}
			catch
			{
				// Log.Error($"Failed to update task [{id}].");
				// Output.WriteError($"Failed to update task details. Please try again.");
			}
		}
	}
}