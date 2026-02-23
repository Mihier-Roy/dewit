using Dewit.Core.Entities;

namespace Dewit.Core.Interfaces
{
    public interface ITaskService
    {
        IEnumerable<TaskItem> GetTasks(
            string sort = "date",
            string duration = "today",
            string? status = null,
            string? tags = null,
            string? search = null
        );

        void AddTask(string title, string status, string? tags = null, string? recur = null);

        void DeleteTask(int id);

        TaskItem? CompleteTask(int id, string completedAt);

        TaskItem UpdateTaskDetails(
            int id,
            string? title = null,
            string? addTags = null,
            string? removeTags = null,
            bool resetTags = false,
            string? recur = null,
            bool removeRecur = false
        );

        void ImportTask(TaskItem taskItem);
    }
}