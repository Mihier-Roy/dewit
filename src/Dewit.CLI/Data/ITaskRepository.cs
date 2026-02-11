using System.Collections.Generic;
using Dewit.CLI.Models;

namespace Dewit.CLI.Data
{
    public interface ITaskRepository
    {
        IEnumerable<TaskItem> GetTasks();
        TaskItem GetTaskById(int id);
        void AddTask(TaskItem task);
        void UpdateTask(TaskItem task);
        void RemoveTask(TaskItem task);
        bool SaveChanges();
    }
}
