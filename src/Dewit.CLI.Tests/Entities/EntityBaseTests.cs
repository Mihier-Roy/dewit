using Dewit.Core.Entities;

namespace Dewit.CLI.Tests.Entities;

public class EntityBaseTests
{
    [Test]
    public async Task TaskItem_InheritsFromEntityBase()
    {
        var task = new TaskItem
        {
            TaskDescription = "Test task",
            Status = "Doing",
            AddedOn = DateTime.Now,
        };

        EntityBase entityBase = task; // Should compile if TaskItem inherits from EntityBase
        await Assert.That(entityBase).IsNotNull();
        await Assert.That(task is EntityBase).IsTrue();
    }

    [Test]
    public async Task TaskItem_HasIdProperty()
    {
        var task = new TaskItem
        {
            TaskDescription = "Test task",
            Status = "Doing",
            AddedOn = DateTime.Now,
        };

        await Assert.That(task.Id).IsTypeOf<int>();
    }

    [Test]
    public async Task TaskItem_DefaultIdIsZero()
    {
        var task = new TaskItem
        {
            TaskDescription = "Test task",
            Status = "Doing",
            AddedOn = DateTime.Now,
        };

        await Assert.That(task.Id).IsEqualTo(0);
    }

    [Test]
    public async Task NewTaskItem_CanSetAllCoreProperties()
    {
        var now = DateTime.Now;
        var task = new TaskItem
        {
            TaskDescription = "Test task from Core",
            Status = "Doing",
            Tags = "test,core",
            AddedOn = now,
            CompletedOn = DateTime.MinValue,
        };

        await Assert.That(task.TaskDescription).IsEqualTo("Test task from Core");
        await Assert.That(task.Status).IsEqualTo("Doing");
        await Assert.That(task.Tags).IsEqualTo("test,core");
        await Assert.That(task.AddedOn).IsEqualTo(now);
    }
}
