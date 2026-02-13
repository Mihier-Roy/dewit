using Dewit.Core.Entities;

namespace Dewit.CLI.Tests;

public class TaskItemTests
{
    [Test]
    public async Task TaskItem_CanSetAndGetAllProperties()
    {
        var now = DateTime.Now;
        var task = new TaskItem
        {
            TaskDescription = "Test task",
            Status = "Doing",
            Tags = "work,test",
            AddedOn = now,
            CompletedOn = DateTime.MinValue
        };

        await Assert.That(task.TaskDescription).IsEqualTo("Test task");
        await Assert.That(task.Status).IsEqualTo("Doing");
        await Assert.That(task.Tags).IsEqualTo("work,test");
        await Assert.That(task.AddedOn).IsEqualTo(now);
        await Assert.That(task.CompletedOn).IsEqualTo(DateTime.MinValue);
    }

    [Test]
    public async Task TaskItem_IdDefaultsToZero()
    {
        var task = new TaskItem
        {
            TaskDescription = "New task",
            Status = "Later"
        };

        await Assert.That(task.Id).IsEqualTo(0);
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
