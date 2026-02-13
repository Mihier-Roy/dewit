using Dewit.Core.Entities;

namespace Dewit.Core.Interfaces
{
    public interface ITaskService
    {
        IEnumerable<TaskItem> GetTasks(string sort = "date",
            string duration = "today",
            string? status = null,
            string? tags = null,
            string? search = null);

        void AddTask(string title, string status, string? tags = null);

        void DeleteTask(int id);

        void CompleteTask(int id, string completedAt);

        TaskItem UpdateTaskDetails(int id, string? title = null,
            string? addTags = null,
            string? removeTags = null,
            bool resetTags = false);

        void ImportTask(TaskItem taskItem);
    }
}
