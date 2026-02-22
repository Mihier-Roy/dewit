using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Dewit.Core.Services;
using Dewit.Data.Data;
using Dewit.Data.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dewit.CLI.Tests.Services;

public class MoodServiceTests
{
    private DewitDbContext _context = null!;
    private IRepository<MoodEntry> _moodRepo = null!;
    private IRepository<MoodDescriptorItem> _descriptorRepo = null!;
    private IMoodService _service = null!;

    [Before(Test)]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DewitDbContext>()
            .UseInMemoryDatabase($"MoodTest_{Guid.NewGuid()}")
            .Options;
        _context = new DewitDbContext(options);
        _moodRepo = new Repository<MoodEntry>(_context);
        _descriptorRepo = new Repository<MoodDescriptorItem>(_context);
        _service = new MoodService(_moodRepo, _descriptorRepo);
    }

    [After(Test)]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [Test]
    public async Task AddEntry_CreatesEntry()
    {
        _service.AddEntry("Happy", "calm,focused", DateTime.Today);
        var entry = _service.GetEntryForDate(DateTime.Today);

        await Assert.That(entry).IsNotNull();
        await Assert.That(entry!.Mood).IsEqualTo("Happy");
        await Assert.That(entry.Descriptors).IsEqualTo("calm,focused");
    }

    [Test]
    public async Task AddEntry_ThrowsIfDuplicateDate()
    {
        _service.AddEntry("Happy", "calm", DateTime.Today);

        await Assert
            .That(() => _service.AddEntry("Meh", "tired", DateTime.Today))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task UpdateEntry_UpdatesMoodAndDescriptors()
    {
        _service.AddEntry("Happy", "calm", DateTime.Today);
        _service.UpdateEntry(DateTime.Today, "Meh", "tired,bored");

        var entry = _service.GetEntryForDate(DateTime.Today);
        await Assert.That(entry!.Mood).IsEqualTo("Meh");
        await Assert.That(entry.Descriptors).IsEqualTo("tired,bored");
    }

    [Test]
    public async Task UpdateEntry_UpdatesMoodOnly_KeepsDescriptors()
    {
        _service.AddEntry("Happy", "calm,focused", DateTime.Today);
        _service.UpdateEntry(DateTime.Today, "Meh", null);

        var entry = _service.GetEntryForDate(DateTime.Today);
        await Assert.That(entry!.Mood).IsEqualTo("Meh");
        await Assert.That(entry.Descriptors).IsEqualTo("calm,focused");
    }

    [Test]
    public async Task UpdateEntry_ThrowsIfNoEntryExists()
    {
        await Assert
            .That(() => _service.UpdateEntry(DateTime.Today, "Meh", null))
            .Throws<ApplicationException>();
    }

    [Test]
    public async Task GetEntryForDate_ReturnsNullIfNone()
    {
        var entry = _service.GetEntryForDate(DateTime.Today);
        await Assert.That(entry).IsNull();
    }

    [Test]
    public async Task GetEntriesInRange_ReturnsCorrectEntries()
    {
        _service.AddEntry("Happy", "calm", DateTime.Today);
        _service.AddEntry("Meh", "tired", DateTime.Today.AddDays(-1));
        _service.AddEntry("Down", "stressed", DateTime.Today.AddDays(-10));

        var range = _service.GetEntriesInRange(DateTime.Today.AddDays(-2), DateTime.Today).ToList();

        await Assert.That(range.Count).IsEqualTo(2);
    }

    [Test]
    public async Task GetDescriptors_ReturnsConfigValues()
    {
        // Seed a descriptor entry manually
        _descriptorRepo.Add(
            new MoodDescriptorItem { Mood = "Happy", Descriptors = "calm,focused,cheerful" }
        );

        var descriptors = _service.GetDescriptors("Happy").ToList();

        await Assert.That(descriptors.Count).IsEqualTo(3);
        await Assert.That(descriptors).Contains("calm");
    }

    [Test]
    public async Task GetDescriptors_ReturnsEmptyIfNotConfigured()
    {
        var descriptors = _service.GetDescriptors("Happy").ToList();
        await Assert.That(descriptors.Count).IsEqualTo(0);
    }
}