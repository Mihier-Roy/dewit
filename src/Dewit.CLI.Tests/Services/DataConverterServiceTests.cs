using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Dewit.Core.Services;
using Microsoft.Extensions.Logging;

namespace Dewit.CLI.Tests.Services;

public class DataConverterServiceTests
{
    private IDataConverter _converter = null!;
    private ILogger<DataConverterService> _logger = null!;
    private string _testDirectory = null!;

    [Before(Test)]
    public void Setup()
    {
        _logger = LoggerFactory.Create(b => { }).CreateLogger<DataConverterService>();
        _converter = new DataConverterService(_logger);
        _testDirectory = Path.Combine(Path.GetTempPath(), $"DewitTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [After(Test)]
    public void Cleanup()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    [Test]
    public async Task ExportToJson_CreatesFile()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem { TaskDescription = "Test 1", Status = "Doing", AddedOn = DateTime.Now },
            new TaskItem { TaskDescription = "Test 2", Status = "Done", AddedOn = DateTime.Now }
        };
        var filePath = Path.Combine(_testDirectory, "test.json");

        _converter.ExportToFile(tasks, filePath, DataFormats.Json);

        await Assert.That(File.Exists(filePath)).IsTrue();
    }

    [Test]
    public async Task ExportToCsv_CreatesFile()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem { TaskDescription = "Test 1", Status = "Doing", AddedOn = DateTime.Now },
            new TaskItem { TaskDescription = "Test 2", Status = "Done", AddedOn = DateTime.Now }
        };
        var filePath = Path.Combine(_testDirectory, "test.csv");

        _converter.ExportToFile(tasks, filePath, DataFormats.Csv);

        await Assert.That(File.Exists(filePath)).IsTrue();
    }

    [Test]
    public async Task ImportFromJson_ReturnsData()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem { TaskDescription = "Test 1", Status = "Doing", Tags = "test", AddedOn = DateTime.Now },
            new TaskItem { TaskDescription = "Test 2", Status = "Done", Tags = "work", AddedOn = DateTime.Now }
        };
        var filePath = Path.Combine(_testDirectory, "test.json");
        _converter.ExportToFile(tasks, filePath, DataFormats.Json);

        var imported = _converter.ImportFromFile<TaskItem>(filePath, DataFormats.Json).ToList();

        await Assert.That(imported.Count).IsEqualTo(2);
        await Assert.That(imported[0].TaskDescription).IsEqualTo("Test 1");
        await Assert.That(imported[1].TaskDescription).IsEqualTo("Test 2");
    }

    [Test]
    public async Task ImportFromCsv_ReturnsData()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem { TaskDescription = "CSV Test 1", Status = "Doing", Tags = "csv", AddedOn = DateTime.Now },
            new TaskItem { TaskDescription = "CSV Test 2", Status = "Later", Tags = "test", AddedOn = DateTime.Now }
        };
        var filePath = Path.Combine(_testDirectory, "test.csv");
        _converter.ExportToFile(tasks, filePath, DataFormats.Csv);

        var imported = _converter.ImportFromFile<TaskItem>(filePath, DataFormats.Csv).ToList();

        await Assert.That(imported.Count).IsEqualTo(2);
        await Assert.That(imported[0].TaskDescription).IsEqualTo("CSV Test 1");
        await Assert.That(imported[1].TaskDescription).IsEqualTo("CSV Test 2");
    }

    [Test]
    public async Task ImportFromJson_ThrowsForNonExistentFile()
    {
        var filePath = Path.Combine(_testDirectory, "nonexistent.json");

        await Assert.That(() => _converter.ImportFromFile<TaskItem>(filePath, DataFormats.Json))
            .Throws<FileNotFoundException>();
    }

    [Test]
    public async Task RoundTrip_Json_PreservesData()
    {
        var original = new List<TaskItem>
        {
            new TaskItem
            {
                TaskDescription = "Round trip test",
                Status = "Doing",
                Tags = "test,roundtrip",
                AddedOn = new DateTime(2026, 1, 1, 12, 0, 0),
                CompletedOn = DateTime.MinValue
            }
        };
        var filePath = Path.Combine(_testDirectory, "roundtrip.json");

        _converter.ExportToFile(original, filePath, DataFormats.Json);
        var imported = _converter.ImportFromFile<TaskItem>(filePath, DataFormats.Json).ToList();

        await Assert.That(imported[0].TaskDescription).IsEqualTo(original[0].TaskDescription);
        await Assert.That(imported[0].Status).IsEqualTo(original[0].Status);
        await Assert.That(imported[0].Tags).IsEqualTo(original[0].Tags);
    }

    [Test]
    public async Task RoundTrip_Csv_PreservesData()
    {
        var original = new List<TaskItem>
        {
            new TaskItem
            {
                TaskDescription = "CSV round trip",
                Status = "Later",
                Tags = "csv,test",
                AddedOn = new DateTime(2026, 1, 1, 12, 0, 0),
                CompletedOn = DateTime.MinValue
            }
        };
        var filePath = Path.Combine(_testDirectory, "roundtrip.csv");

        _converter.ExportToFile(original, filePath, DataFormats.Csv);
        var imported = _converter.ImportFromFile<TaskItem>(filePath, DataFormats.Csv).ToList();

        await Assert.That(imported[0].TaskDescription).IsEqualTo(original[0].TaskDescription);
        await Assert.That(imported[0].Status).IsEqualTo(original[0].Status);
        await Assert.That(imported[0].Tags).IsEqualTo(original[0].Tags);
    }
}
