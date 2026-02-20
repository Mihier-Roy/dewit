using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Dewit.Core.Utils;
using Dewit.Data.Data;
using Dewit.Data.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dewit.CLI.Tests.Utils;

public class MoodDescriptorDefaultsTests
{
    private DewitDbContext _context = null!;
    private IRepository<MoodDescriptorItem> _repo = null!;

    [Before(Test)]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DewitDbContext>()
            .UseInMemoryDatabase($"DescriptorDefaultsTest_{Guid.NewGuid()}")
            .Options;
        _context = new DewitDbContext(options);
        _repo = new Repository<MoodDescriptorItem>(_context);
    }

    [After(Test)]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [Test]
    public async Task SeedIfMissing_WhenEmpty_SeedsAllMoods()
    {
        MoodDescriptorDefaults.SeedIfMissing(_repo);

        var items = _repo.List().ToList();
        await Assert.That(items.Count).IsEqualTo(Enum.GetValues<Mood>().Length);
    }

    [Test]
    public async Task SeedIfMissing_WhenEmpty_EachMoodHasDescriptors()
    {
        MoodDescriptorDefaults.SeedIfMissing(_repo);

        var items = _repo.List().ToList();
        foreach (var item in items)
        {
            await Assert.That(item.Descriptors).IsNotNull();
            await Assert.That(item.Descriptors.Length).IsGreaterThan(0);
        }
    }

    [Test]
    public async Task SeedIfMissing_WhenAlreadySeeded_DoesNotAddMore()
    {
        MoodDescriptorDefaults.SeedIfMissing(_repo);
        MoodDescriptorDefaults.SeedIfMissing(_repo);

        var items = _repo.List().ToList();
        await Assert.That(items.Count).IsEqualTo(Enum.GetValues<Mood>().Length);
    }

    [Test]
    public async Task SeedIfMissing_WhenRepoHasAnyEntry_DoesNotSeed()
    {
        _repo.Add(new MoodDescriptorItem { Mood = "Happy", Descriptors = "calm,focused" });

        MoodDescriptorDefaults.SeedIfMissing(_repo);

        var items = _repo.List().ToList();
        await Assert.That(items.Count).IsEqualTo(1);
    }

    [Test]
    public async Task SeedIfMissing_SeededEntriesMatchAllMoodNames()
    {
        MoodDescriptorDefaults.SeedIfMissing(_repo);

        var seededMoods = _repo
            .List()
            .Select(i => i.Mood)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var mood in Enum.GetValues<Mood>())
        {
            await Assert.That(seededMoods.Contains(mood.ToString())).IsTrue();
        }
    }
}
