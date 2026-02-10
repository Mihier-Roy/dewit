using Dewit.CLI.Models;
using Dewit.CLI.Utils;

namespace Dewit.CLI.Tests;

public class FormatDataTests
{
    private string _tempDir = null!;

    [Before(Test)]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"dewit_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    [After(Test)]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private static List<TaskItem> CreateSampleTasks()
    {
        return new List<TaskItem>
        {
            new TaskItem
            {
                Id = 1,
                TaskDescription = "First task",
                Status = "Doing",
                Tags = "work,dev",
                AddedOn = new DateTime(2024, 1, 15, 10, 30, 0),
                CompletedOn = DateTime.MinValue
            },
            new TaskItem
            {
                Id = 2,
                TaskDescription = "Second task",
                Status = "Done",
                Tags = "test",
                AddedOn = new DateTime(2024, 1, 16, 14, 0, 0),
                CompletedOn = new DateTime(2024, 1, 17, 9, 0, 0)
            }
        };
    }

    [Test]
    public async Task ToType_Json_CreatesValidJsonFile()
    {
        var tasks = CreateSampleTasks();
        var filePath = Path.Combine(_tempDir, "tasks.json");

        FormatData.ToType(tasks, filePath, "json");

        await Assert.That(File.Exists(filePath)).IsTrue();
        var content = File.ReadAllText(filePath);
        await Assert.That(content).IsNotEmpty();
    }

    [Test]
    public async Task ToType_Csv_CreatesValidCsvFile()
    {
        var tasks = CreateSampleTasks();
        var filePath = Path.Combine(_tempDir, "tasks.csv");

        FormatData.ToType(tasks, filePath, "csv");

        await Assert.That(File.Exists(filePath)).IsTrue();
        var content = File.ReadAllText(filePath);
        await Assert.That(content).Contains("First task");
        await Assert.That(content).Contains("Second task");
    }

    [Test]
    public async Task Json_RoundTrip_PreservesData()
    {
        var tasks = CreateSampleTasks();
        var filePath = Path.Combine(_tempDir, "roundtrip.json");

        FormatData.ToType(tasks, filePath, "json");
        var loaded = FormatData.FromType(filePath, "json").ToList();

        await Assert.That(loaded.Count).IsEqualTo(2);
        await Assert.That(loaded[0].TaskDescription).IsEqualTo("First task");
        await Assert.That(loaded[0].Status).IsEqualTo("Doing");
        await Assert.That(loaded[0].Tags).IsEqualTo("work,dev");
        await Assert.That(loaded[1].TaskDescription).IsEqualTo("Second task");
        await Assert.That(loaded[1].Status).IsEqualTo("Done");
    }

    [Test]
    public async Task Csv_RoundTrip_PreservesData()
    {
        var tasks = CreateSampleTasks();
        var filePath = Path.Combine(_tempDir, "roundtrip.csv");

        FormatData.ToType(tasks, filePath, "csv");
        var loaded = FormatData.FromType(filePath, "csv").ToList();

        await Assert.That(loaded.Count).IsEqualTo(2);
        await Assert.That(loaded[0].TaskDescription).IsEqualTo("First task");
        await Assert.That(loaded[0].Status).IsEqualTo("Doing");
        await Assert.That(loaded[1].TaskDescription).IsEqualTo("Second task");
        await Assert.That(loaded[1].Status).IsEqualTo("Done");
    }

    [Test]
    public void FromType_InvalidType_ThrowsException()
    {
        var filePath = Path.Combine(_tempDir, "tasks.txt");
        File.WriteAllText(filePath, "dummy");

        Assert.Throws<Exception>(() => FormatData.FromType(filePath, "xml"));
    }

    [Test]
    public async Task ToType_EmptyCollection_CreatesFile()
    {
        var tasks = new List<TaskItem>();
        var filePath = Path.Combine(_tempDir, "empty.json");

        FormatData.ToType(tasks, filePath, "json");

        await Assert.That(File.Exists(filePath)).IsTrue();
    }

    [Test]
    public async Task Json_RoundTrip_EmptyCollection_ReturnsEmpty()
    {
        var tasks = new List<TaskItem>();
        var filePath = Path.Combine(_tempDir, "empty.json");

        FormatData.ToType(tasks, filePath, "json");
        var loaded = FormatData.FromType(filePath, "json").ToList();

        await Assert.That(loaded.Count).IsEqualTo(0);
    }
}
