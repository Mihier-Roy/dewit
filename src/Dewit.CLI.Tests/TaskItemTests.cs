using Dewit.CLI.Models;

namespace Dewit.CLI.Tests;

public class TaskItemTests
{
    [Test]
    public async Task TaskItem_CanSetAndGetAllProperties()
    {
        var now = DateTime.Now;
        var task = new TaskItem
        {
            Id = 1,
            TaskDescription = "Test task",
            Status = "Doing",
            Tags = "work,test",
            AddedOn = now,
            CompletedOn = DateTime.MinValue
        };

        await Assert.That(task.Id).IsEqualTo(1);
        await Assert.That(task.TaskDescription).IsEqualTo("Test task");
        await Assert.That(task.Status).IsEqualTo("Doing");
        await Assert.That(task.Tags).IsEqualTo("work,test");
        await Assert.That(task.AddedOn).IsEqualTo(now);
        await Assert.That(task.CompletedOn).IsEqualTo(DateTime.MinValue);
    }

    [Test]
    public async Task TaskItem_IdCanBeNull()
    {
        var task = new TaskItem
        {
            Id = null,
            TaskDescription = "New task",
            Status = "Later"
        };

        await Assert.That(task.Id).IsNull();
    }

    [Test]
    [Arguments("Doing")]
    [Arguments("Done")]
    [Arguments("Later")]
    public async Task TaskItem_AcceptsValidStatuses(string status)
    {
        var task = new TaskItem
        {
            TaskDescription = "Test",
            Status = status
        };

        await Assert.That(task.Status).IsEqualTo(status);
    }

    [Test]
    public async Task TaskItem_DefaultDateTimeIsMinValue()
    {
        var task = new TaskItem
        {
            TaskDescription = "Test",
            Status = "Doing"
        };

        await Assert.That(task.CompletedOn).IsEqualTo(DateTime.MinValue);
    }

    [Test]
    public async Task TaskItem_TagsCanBeNull()
    {
        var task = new TaskItem
        {
            TaskDescription = "Test",
            Status = "Doing",
            Tags = null!
        };

        await Assert.That(task.Tags).IsNull();
    }
}
