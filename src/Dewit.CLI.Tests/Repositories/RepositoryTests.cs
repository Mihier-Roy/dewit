using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Dewit.Data.Data;
using Dewit.Data.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dewit.CLI.Tests.Repositories;

public class RepositoryTests
{
    private DewitDbContext _context = null!;
    private IRepository<TaskItem> _repository = null!;

    [Before(Test)]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DewitDbContext>()
            .UseInMemoryDatabase($"Test_{Guid.NewGuid()}")
            .Options;
        _context = new DewitDbContext(options);
        _repository = new Repository<TaskItem>(_context);
    }

    [After(Test)]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [Test]
    public async Task Add_CreatesNewTaskItem()
    {
        var task = new TaskItem
        {
            TaskDescription = "Test",
            Status = "Doing",
            AddedOn = DateTime.Now,
        };

        _repository.Add(task);
        var tasks = _repository.List();

        await Assert.That(tasks.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task Add_AssignsIdToNewTask()
    {
        var task = new TaskItem
        {
            TaskDescription = "Test",
            Status = "Doing",
            AddedOn = DateTime.Now,
        };

        _repository.Add(task);

        await Assert.That(task.Id).IsGreaterThan(0);
    }

    [Test]
    public async Task GetById_ReturnsCorrectTask()
    {
        var task = new TaskItem
        {
            TaskDescription = "Test",
            Status = "Doing",
            AddedOn = DateTime.Now,
        };
        _repository.Add(task);

        var retrieved = _repository.GetById(task.Id);

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved!.TaskDescription).IsEqualTo("Test");
    }

    [Test]
    public async Task GetById_ReturnsNullForInvalidId()
    {
        var retrieved = _repository.GetById(999);

        await Assert.That(retrieved).IsNull();
    }

    [Test]
    public async Task Update_ModifiesExistingTask()
    {
        var task = new TaskItem
        {
            TaskDescription = "Original",
            Status = "Doing",
            AddedOn = DateTime.Now,
        };
        _repository.Add(task);

        task.TaskDescription = "Updated";
        _repository.Update(task);

        var updated = _repository.GetById(task.Id);
        await Assert.That(updated!.TaskDescription).IsEqualTo("Updated");
    }

    [Test]
    public async Task Remove_DeletesTask()
    {
        var task = new TaskItem
        {
            TaskDescription = "Test",
            Status = "Doing",
            AddedOn = DateTime.Now,
        };
        _repository.Add(task);

        _repository.Remove(task);

        var tasks = _repository.List();
        await Assert.That(tasks.Count()).IsEqualTo(0);
    }

    [Test]
    public async Task List_ReturnsAllTasks()
    {
        var task1 = new TaskItem
        {
            TaskDescription = "Task 1",
            Status = "Doing",
            AddedOn = DateTime.Now,
        };
        var task2 = new TaskItem
        {
            TaskDescription = "Task 2",
            Status = "Later",
            AddedOn = DateTime.Now,
        };

        _repository.Add(task1);
        _repository.Add(task2);

        var tasks = _repository.List();

        await Assert.That(tasks.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task List_ReturnsEmptyWhenNoTasks()
    {
        var tasks = _repository.List();

        await Assert.That(tasks.Count()).IsEqualTo(0);
    }

    [Test]
    public async Task Multiple_AddAndRemove_WorksCorrectly()
    {
        // Add multiple tasks
        for (int i = 0; i < 5; i++)
        {
            _repository.Add(
                new TaskItem
                {
                    TaskDescription = $"Task {i}",
                    Status = "Doing",
                    AddedOn = DateTime.Now,
                }
            );
        }

        var allTasks = _repository.List().ToList();
        await Assert.That(allTasks.Count).IsEqualTo(5);

        // Remove some tasks
        _repository.Remove(allTasks[0]);
        _repository.Remove(allTasks[2]);

        var remaining = _repository.List();
        await Assert.That(remaining.Count()).IsEqualTo(3);
    }
}