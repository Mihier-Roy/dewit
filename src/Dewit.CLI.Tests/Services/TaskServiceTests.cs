using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Dewit.Core.Services;
using Dewit.Data.Data;
using Dewit.Data.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dewit.CLI.Tests.Services;

public class TaskServiceTests
{
    private DewitDbContext _context = null!;
    private IRepository<TaskItem> _repository = null!;
    private ITaskService _service = null!;
    private ILogger<TaskService> _logger = null!;

    [Before(Test)]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DewitDbContext>()
            .UseInMemoryDatabase($"Test_{Guid.NewGuid()}")
            .Options;
        _context = new DewitDbContext(options);
        _repository = new Repository<TaskItem>(_context);
        _logger = LoggerFactory.Create(b => { }).CreateLogger<TaskService>();
        _service = new TaskService(_repository, _logger);
    }

    [After(Test)]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [Test]
    public async Task AddTask_CreatesNewTask()
    {
        _service.AddTask("Test task", "Doing", "test,work");
        var tasks = _service.GetTasks(duration: "all");

        await Assert.That(tasks.Count()).IsEqualTo(1);
        await Assert.That(tasks.First().TaskDescription).IsEqualTo("Test task");
    }

    [Test]
    public async Task AddTask_SanitizesTags()
    {
        _service.AddTask("Test", "Doing", "TEST, work, TEST");
        var tasks = _service.GetTasks(duration: "all").ToList();

        await Assert.That(tasks[0].Tags).IsEqualTo("test,work");
    }

    [Test]
    public async Task AddTask_ThrowsExceptionForEmptyTitle()
    {
        await Assert.That(() => _service.AddTask("", "Doing", null)).Throws<ArgumentException>();
    }

    [Test]
    public async Task GetTasks_FiltersByDuration_Today()
    {
        _service.AddTask("Today task", "Doing", null);
        var tasks = _service.GetTasks(duration: "today");

        await Assert.That(tasks.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task GetTasks_FiltersByDuration_All()
    {
        _service.AddTask("Task 1", "Doing", null);
        _service.AddTask("Task 2", "Later", null);

        var tasks = _service.GetTasks(duration: "all");

        await Assert.That(tasks.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task GetTasks_FiltersByStatus()
    {
        _service.AddTask("Task 1", "Doing", null);
        _service.AddTask("Task 2", "Later", null);
        _service.AddTask("Task 3", "Doing", null);

        var doingTasks = _service.GetTasks(duration: "all", status: "doing");

        await Assert.That(doingTasks.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task GetTasks_FiltersBySearch()
    {
        _service.AddTask("Buy groceries", "Doing", null);
        _service.AddTask("Buy tickets", "Later", null);
        _service.AddTask("Write code", "Doing", null);

        var buyTasks = _service.GetTasks(duration: "all", search: "Buy");

        await Assert.That(buyTasks.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task GetTasks_FiltersByTags()
    {
        _service.AddTask("Task 1", "Doing", "work,urgent");
        _service.AddTask("Task 2", "Later", "personal");
        _service.AddTask("Task 3", "Doing", "work");

        var workTasks = _service.GetTasks(duration: "all", tags: "work");

        await Assert.That(workTasks.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task CompleteTask_SetsStatusToDone()
    {
        _service.AddTask("Task", "Doing", null);
        var tasks = _service.GetTasks(duration: "all").ToList();

        _service.CompleteTask(tasks[0].Id, string.Empty);

        var updated = _repository.GetById(tasks[0].Id);
        await Assert.That(updated!.Status).IsEqualTo("Done");
        await Assert.That(updated.CompletedOn).IsGreaterThan(DateTime.MinValue);
    }

    [Test]
    public async Task CompleteTask_ThrowsExceptionForInvalidId()
    {
        await Assert
            .That(() => _service.CompleteTask(999, string.Empty))
            .Throws<ApplicationException>();
    }

    [Test]
    public async Task UpdateTaskDetails_UpdatesTitle()
    {
        _service.AddTask("Original", "Doing", null);
        var tasks = _service.GetTasks(duration: "all").ToList();

        _service.UpdateTaskDetails(tasks[0].Id, title: "Updated");

        var updated = _repository.GetById(tasks[0].Id);
        await Assert.That(updated!.TaskDescription).IsEqualTo("Updated");
    }

    [Test]
    public async Task UpdateTaskDetails_AddsTags()
    {
        _service.AddTask("Task", "Doing", "existing");
        var tasks = _service.GetTasks(duration: "all").ToList();

        _service.UpdateTaskDetails(tasks[0].Id, addTags: "new");

        var updated = _repository.GetById(tasks[0].Id);
        await Assert.That(updated!.Tags).Contains("new");
        await Assert.That(updated.Tags).Contains("existing");
    }

    [Test]
    public async Task UpdateTaskDetails_RemovesTags()
    {
        _service.AddTask("Task", "Doing", "tag1,tag2,tag3");
        var tasks = _service.GetTasks(duration: "all").ToList();

        _service.UpdateTaskDetails(tasks[0].Id, removeTags: "tag2");

        var updated = _repository.GetById(tasks[0].Id);
        await Assert.That(updated!.Tags).DoesNotContain("tag2");
        await Assert.That(updated.Tags).Contains("tag1");
        await Assert.That(updated.Tags).Contains("tag3");
    }

    [Test]
    public async Task UpdateTaskDetails_ResetsTags()
    {
        _service.AddTask("Task", "Doing", "tag1,tag2");
        var tasks = _service.GetTasks(duration: "all").ToList();

        _service.UpdateTaskDetails(tasks[0].Id, resetTags: true);

        var updated = _repository.GetById(tasks[0].Id);
        await Assert.That(updated!.Tags).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task UpdateTaskDetails_ThrowsExceptionForInvalidId()
    {
        await Assert
            .That(() => _service.UpdateTaskDetails(999, title: "New"))
            .Throws<ApplicationException>();
    }

    [Test]
    public async Task DeleteTask_RemovesTask()
    {
        _service.AddTask("Task", "Doing", null);
        var tasks = _service.GetTasks(duration: "all").ToList();

        _service.DeleteTask(tasks[0].Id);

        var remaining = _service.GetTasks(duration: "all");
        await Assert.That(remaining.Count()).IsEqualTo(0);
    }

    [Test]
    public async Task DeleteTask_ThrowsExceptionForInvalidId()
    {
        await Assert.That(() => _service.DeleteTask(999)).Throws<ApplicationException>();
    }

    [Test]
    public async Task ImportTask_AddsTask()
    {
        var task = new TaskItem
        {
            TaskDescription = "Imported task",
            Status = "Later",
            Tags = "imported",
            AddedOn = DateTime.Now.AddDays(-1),
        };

        _service.ImportTask(task);

        var tasks = _service.GetTasks(duration: "all");
        await Assert.That(tasks.Count()).IsEqualTo(1);
        await Assert.That(tasks.First().TaskDescription).IsEqualTo("Imported task");
    }
}
