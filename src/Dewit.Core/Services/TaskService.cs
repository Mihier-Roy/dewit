using System.Globalization;
using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Dewit.Core.Utils;

namespace Dewit.Core.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<TaskItem> _repository;

        public TaskService(IRepository<TaskItem> repository)
        {
            _repository = repository;
        }

        public IEnumerable<TaskItem> GetTasks(
            string sort = "date",
            string duration = "today",
            string? status = null,
            string? tags = null,
            string? search = null
        )
        {
            var tasks = _repository.List().AsEnumerable();

            // Filter by duration
            tasks = duration switch
            {
                "yesterday" => tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-1)),
                "today" => tasks.Where(p => p.AddedOn.Date == DateTime.Today.Date),
                "week" => tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-7)),
                "month" => tasks.Where(p => p.AddedOn.Date > DateTime.Today.AddDays(-30)),
                _ => tasks,
            };

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                var statusFilter = status.ToLower() switch
                {
                    "doing" => "Doing",
                    "done" => "Done",
                    "later" => "Later",
                    _ => null,
                };

                if (statusFilter != null)
                    tasks = tasks.Where(p => p.Status == statusFilter);
            }

            // Filter by search
            if (!string.IsNullOrEmpty(search))
            {
                tasks = tasks.Where(p =>
                    p.TaskDescription.Contains(search, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Filter by tags
            if (!string.IsNullOrEmpty(tags))
            {
                tags = Sanitizer.SanitizeTags(tags);
                var requestedTags = tags.Split(',');
                tasks = tasks.Where(p => requestedTags.All(tag => p.Tags.Contains(tag)));
            }

            // Sort
            tasks = sort switch
            {
                "status" => tasks.OrderBy(p => p.Status),
                _ => tasks.OrderByDescending(p => p.AddedOn),
            };

            return tasks;
        }

        public void AddTask(string title, string status, string? tags = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title cannot be empty", nameof(title));

            // Sanitize and deduplicate tags
            if (!string.IsNullOrEmpty(tags))
            {
                tags = Sanitizer.SanitizeTags(tags);
                tags = Sanitizer.DeduplicateTags(tags);
            }

            var newTask = new TaskItem
            {
                TaskDescription = title,
                AddedOn = DateTime.Now,
                Status = status,
                Tags = tags ?? string.Empty,
            };

            try
            {
                _repository.Add(newTask);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to add task", ex);
            }
        }

        public void DeleteTask(int id)
        {
            var task = _repository.GetById(id);
            if (task == null)
                throw new ApplicationException($"Task with ID {id} does not exist");

            try
            {
                _repository.Remove(task);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to delete task with ID {id}", ex);
            }
        }

        public void CompleteTask(int id, string completedAt)
        {
            var task = _repository.GetById(id);
            if (task == null)
                throw new ApplicationException($"Task with ID {id} does not exist");

            // Parse the completed date if provided
            if (!string.IsNullOrEmpty(completedAt))
            {
                var culture = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
                var styles = DateTimeStyles.AssumeLocal;

                if (DateTime.TryParse(completedAt, culture, styles, out DateTime completedOn))
                {
                    task.CompletedOn = completedOn;
                }
                else
                {
                    throw new ArgumentException(
                        $"Invalid date format: {completedAt}",
                        nameof(completedAt)
                    );
                }
            }
            else
            {
                task.CompletedOn = DateTime.Now;
            }

            task.Status = "Done";

            try
            {
                _repository.Update(task);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to complete task with ID {id}", ex);
            }
        }

        public TaskItem UpdateTaskDetails(
            int id,
            string? title = null,
            string? addTags = null,
            string? removeTags = null,
            bool resetTags = false
        )
        {
            var task = _repository.GetById(id);
            if (task == null)
                throw new ApplicationException($"Task with ID {id} does not exist");

            // Update title
            if (!string.IsNullOrEmpty(title))
            {
                task.TaskDescription = title;
            }

            // Reset tags if requested
            if (resetTags)
            {
                task.Tags = string.Empty;
            }
            else
            {
                // Add tags
                if (!string.IsNullOrEmpty(addTags))
                {
                    addTags = Sanitizer.SanitizeTags(addTags);
                    var updatedTags = string.Join(',', task.Tags, addTags);
                    updatedTags = Sanitizer.DeduplicateTags(updatedTags);
                    task.Tags = updatedTags[0] == ',' ? updatedTags.Remove(0, 1) : updatedTags;
                }

                // Remove tags
                if (!string.IsNullOrEmpty(removeTags))
                {
                    var tagsToRemove = Sanitizer.SanitizeTags(removeTags).Split(',');
                    var oldTags = task.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    task.Tags = string.Join(',', oldTags.Except(tagsToRemove));
                }
            }

            try
            {
                _repository.Update(task);
                return task;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to update task with ID {id}", ex);
            }
        }

        public void ImportTask(TaskItem taskItem)
        {
            try
            {
                _repository.Add(taskItem);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to import task", ex);
            }
        }
    }
}
