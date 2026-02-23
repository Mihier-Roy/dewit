using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Dewit.Core.Services;
using Dewit.Data.Data;
using Dewit.Data.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dewit.CLI.Tests.Services;

public class JournalServiceTests
{
    private DewitDbContext _context = null!;
    private IRepository<JournalEntry> _repo = null!;
    private IJournalService _service = null!;
    private string _tempDir = null!;

    [Before(Test)]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"dewit_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);

        var options = new DbContextOptionsBuilder<DewitDbContext>()
            .UseInMemoryDatabase($"JournalTest_{Guid.NewGuid()}")
            .Options;

        _context = new DewitDbContext(options);
        _repo = new Repository<JournalEntry>(_context);
        _service = new JournalService(_repo, _tempDir);
    }

    [After(Test)]
    public void Cleanup()
    {
        _context.Dispose();
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Test]
    public async Task GetFilePath_ReturnsCorrectPath()
    {
        var date = new DateTime(2026, 2, 22);
        var result = _service.GetFilePath(date);
        var expected = Path.Combine(_tempDir, "2026", "02-22.md");

        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task CreateOrGetEntry_CreatesFileAndDbRecord()
    {
        var date = DateTime.Today;
        var entry = _service.CreateOrGetEntry(date, "Happy", "calm,focused");

        await Assert.That(entry).IsNotNull();
        await Assert.That(File.Exists(entry.FilePath)).IsTrue();

        var dbEntry = _service.GetEntryForDate(date);
        await Assert.That(dbEntry).IsNotNull();
        await Assert.That(dbEntry!.FilePath).IsEqualTo(entry.FilePath);
    }

    [Test]
    public async Task CreateOrGetEntry_WritesFrontmatterToFile()
    {
        var date = new DateTime(2026, 3, 15);
        var entry = _service.CreateOrGetEntry(date, "Happy", "calm,focused");

        var content = File.ReadAllText(entry.FilePath);
        await Assert.That(content).Contains("date: 2026-03-15");
        await Assert.That(content).Contains("mood: Happy");
        await Assert.That(content).Contains("mood-descriptors: calm,focused");
    }

    [Test]
    public async Task CreateOrGetEntry_OmitsMoodDescriptorsLineIfEmpty()
    {
        var date = new DateTime(2026, 3, 15);
        var entry = _service.CreateOrGetEntry(date, "Meh", "");

        var content = File.ReadAllText(entry.FilePath);
        await Assert.That(content).Contains("mood: Meh");
        await Assert.That(content).DoesNotContain("mood-descriptors:");
    }

    [Test]
    public async Task CreateOrGetEntry_ExistingEntry_DoesNotOverwriteFile()
    {
        var date = DateTime.Today;
        _service.CreateOrGetEntry(date, "Happy", "calm");

        var filePath = _service.GetFilePath(date);
        File.AppendAllText(filePath, "\nUser's journal text here.");

        _service.CreateOrGetEntry(date, "Meh", "tired");

        var content = File.ReadAllText(filePath);
        await Assert.That(content).Contains("User's journal text here.");
    }

    [Test]
    public async Task CreateOrGetEntry_ExistingEntry_ReturnsSameRecord()
    {
        var date = DateTime.Today;
        var first = _service.CreateOrGetEntry(date, "Happy", "calm");
        var second = _service.CreateOrGetEntry(date, "Meh", "tired");

        await Assert.That(second.Id).IsEqualTo(first.Id);
    }

    [Test]
    public async Task GetEntryForDate_ReturnsNullIfNone()
    {
        var entry = _service.GetEntryForDate(DateTime.Today);
        await Assert.That(entry).IsNull();
    }

    [Test]
    public async Task GetEntryForDate_ReturnsEntryIfExists()
    {
        var date = DateTime.Today;
        _service.CreateOrGetEntry(date, "Happy", "calm");

        var entry = _service.GetEntryForDate(date);
        await Assert.That(entry).IsNotNull();
        await Assert.That(entry!.Date).IsEqualTo(date.Date);
    }

    [Test]
    public async Task GetEntriesInRange_ReturnsCorrectEntries()
    {
        _service.CreateOrGetEntry(DateTime.Today, "Happy", "calm");
        _service.CreateOrGetEntry(DateTime.Today.AddDays(-1), "Meh", "tired");
        _service.CreateOrGetEntry(DateTime.Today.AddDays(-10), "Down", "stressed");

        var range = _service.GetEntriesInRange(DateTime.Today.AddDays(-2), DateTime.Today).ToList();

        await Assert.That(range.Count).IsEqualTo(2);
    }

    [Test]
    public async Task TouchUpdatedAt_UpdatesTimestamp()
    {
        var date = DateTime.Today;
        var entry = _service.CreateOrGetEntry(date, "Happy", "calm");
        var originalTime = entry.UpdatedAt;

        // Small delay to ensure the timestamp differs
        await Task.Delay(10);
        _service.TouchUpdatedAt(date);

        var updated = _service.GetEntryForDate(date);
        await Assert.That(updated!.UpdatedAt).IsGreaterThan(originalTime);
    }

    [Test]
    public async Task TouchUpdatedAt_DoesNothingIfNoEntry()
    {
        // Should not throw
        _service.TouchUpdatedAt(DateTime.Today);
        var entry = _service.GetEntryForDate(DateTime.Today);
        await Assert.That(entry).IsNull();
    }
}
