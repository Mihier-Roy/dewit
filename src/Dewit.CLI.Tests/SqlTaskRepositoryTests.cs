using Dewit.CLI.Data;
using Dewit.CLI.Models;
using Microsoft.EntityFrameworkCore;

namespace Dewit.CLI.Tests;

public class SqlTaskRepositoryTests
{
    private TaskContext _context = null!;
    private SqlTaskRepository _repository = null!;

    [Before(Test)]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<TaskContext>()
            .UseInMemoryDatabase(databaseName: $"DewitTest_{Guid.NewGuid()}")
            .Options;

        _context = new TaskContext(options);
        _repository = new SqlTaskRepository(_context);
    }

    [After(Test)]
    public void TearDown()
    {
        _context.Dispose();
    }

    private TaskItem CreateTask(string description = "Test task", string status = "Doing", string tags = "")
    {
        return new TaskItem
        {
            TaskDescription = description,
            Status = status,
            Tags = tags,
            AddedOn = DateTime.Now
        };
    }

    [Test]
    public async Task AddTask_ValidTask_AddsToDatabase()
    {
        var task = CreateTask();
        _repository.AddTask(task);
        _repository.SaveChanges();

        var tasks = _repository.GetTasks().ToList();
        await Assert.That(tasks.Count).IsEqualTo(1);
        await Assert.That(tasks[0].TaskDescription).IsEqualTo("Test task");
    }

    [Test]
    public void AddTask_NullTask_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _repository.AddTask(null!));
    }

    [Test]
    public async Task GetTaskById_ExistingId_ReturnsTask()
    {
        var task = CreateTask("Find me");
        _repository.AddTask(task);
        _repository.SaveChanges();

        var found = _repository.GetTaskById(task.Id!.Value);
        await Assert.That(found).IsNotNull();
        await Assert.That(found!.TaskDescription).IsEqualTo("Find me");
    }

    [Test]
    public async Task GetTaskById_NonExistentId_ReturnsNull()
    {
        var found = _repository.GetTaskById(999);
        await Assert.That(found).IsNull();
    }

    [Test]
    public async Task GetTasks_ReturnsAllTasks()
    {
        _repository.AddTask(CreateTask("Task 1"));
        _repository.AddTask(CreateTask("Task 2"));
        _repository.AddTask(CreateTask("Task 3"));
        _repository.SaveChanges();

        var tasks = _repository.GetTasks().ToList();
        await Assert.That(tasks.Count).IsEqualTo(3);
    }

    [Test]
    public async Task GetTasks_EmptyDatabase_ReturnsEmpty()
    {
        var tasks = _repository.GetTasks().ToList();
        await Assert.That(tasks.Count).IsEqualTo(0);
    }

    [Test]
    public async Task RemoveTask_ExistingTask_RemovesFromDatabase()
    {
        var task = CreateTask("Delete me");
        _repository.AddTask(task);
        _repository.SaveChanges();

        _repository.RemoveTask(task);
        _repository.SaveChanges();

        var tasks = _repository.GetTasks().ToList();
        await Assert.That(tasks.Count).IsEqualTo(0);
    }

    [Test]
    public void RemoveTask_NullTask_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _repository.RemoveTask(null!));
    }

    [Test]
    public async Task SaveChanges_ReturnsTrue()
    {
        _repository.AddTask(CreateTask());
        var result = _repository.SaveChanges();

        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task UpdateTask_EfCoreTracksChanges()
    {
        var task = CreateTask("Original title");
        _repository.AddTask(task);
        _repository.SaveChanges();

        task.TaskDescription = "Updated title";
        _repository.UpdateTask(task);
        _repository.SaveChanges();

        var updated = _repository.GetTaskById(task.Id!.Value);
        await Assert.That(updated!.TaskDescription).IsEqualTo("Updated title");
    }

    [Test]
    public async Task AddTask_MultipleTasks_AssignsUniqueIds()
    {
        var task1 = CreateTask("Task 1");
        var task2 = CreateTask("Task 2");
        _repository.AddTask(task1);
        _repository.AddTask(task2);
        _repository.SaveChanges();

        await Assert.That(task1.Id).IsNotNull();
        await Assert.That(task2.Id).IsNotNull();
        await Assert.That(task1.Id).IsNotEqualTo(task2.Id);
    }

    [Test]
    public async Task AddTask_WithTags_PersistsTags()
    {
        var task = CreateTask("Tagged task", tags: "work,urgent");
        _repository.AddTask(task);
        _repository.SaveChanges();

        var found = _repository.GetTaskById(task.Id!.Value);
        await Assert.That(found!.Tags).IsEqualTo("work,urgent");
    }

    [Test]
    public async Task UpdateTask_ChangeStatus_Persists()
    {
        var task = CreateTask("Status task", "Doing");
        _repository.AddTask(task);
        _repository.SaveChanges();

        task.Status = "Done";
        task.CompletedOn = DateTime.Now;
        _repository.UpdateTask(task);
        _repository.SaveChanges();

        var updated = _repository.GetTaskById(task.Id!.Value);
        await Assert.That(updated!.Status).IsEqualTo("Done");
        await Assert.That(updated.CompletedOn).IsNotEqualTo(DateTime.MinValue);
    }
}
