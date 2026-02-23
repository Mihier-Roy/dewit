using Dewit.CLI.Utils;
using Dewit.Core.Entities;

namespace Dewit.CLI.Tests.Utils;

public class MoodCalendarTests
{
    private static MoodEntry Entry(DateTime date, string mood = "Happy", string descriptors = "") =>
        new()
        {
            Date = date.Date,
            Mood = mood,
            Descriptors = descriptors,
        };

    [Test]
    public async Task GetDetailEntries_ReturnsEntriesWithDescriptors()
    {
        var today = DateTime.Today;
        var entries = new[]
        {
            Entry(today, descriptors: "calm,focused"),
            Entry(today.AddDays(-1), descriptors: ""),
        };

        var result = MoodCalendar.GetDetailEntries(entries);

        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result[0].Date).IsEqualTo(today.Date);
    }

    [Test]
    public async Task GetDetailEntries_ReturnsEntriesWithJournalDates()
    {
        var today = DateTime.Today;
        var entries = new[]
        {
            Entry(today, descriptors: ""), // no descriptors
            Entry(today.AddDays(-1), descriptors: ""), // no descriptors, no journal
        };
        var journalDates = new HashSet<DateTime> { today };

        var result = MoodCalendar.GetDetailEntries(entries, journalDates);

        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result[0].Date).IsEqualTo(today.Date);
    }

    [Test]
    public async Task GetDetailEntries_ExcludesEntriesWithNeitherDescriptorsNorJournal()
    {
        var today = DateTime.Today;
        var entries = new[]
        {
            Entry(today, descriptors: ""),
            Entry(today.AddDays(-1), descriptors: ""),
        };

        var result = MoodCalendar.GetDetailEntries(entries, journalDates: null);

        await Assert.That(result.Count).IsEqualTo(0);
    }

    [Test]
    public async Task GetDetailEntries_IsOrderedByDateAscending()
    {
        var today = DateTime.Today;
        var entries = new[]
        {
            Entry(today, descriptors: "excited"),
            Entry(today.AddDays(-2), descriptors: "calm"),
            Entry(today.AddDays(-1), descriptors: "focused"),
        };

        var result = MoodCalendar.GetDetailEntries(entries);

        await Assert.That(result[0].Date).IsEqualTo(today.AddDays(-2).Date);
        await Assert.That(result[1].Date).IsEqualTo(today.AddDays(-1).Date);
        await Assert.That(result[2].Date).IsEqualTo(today.Date);
    }

    [Test]
    public async Task GetDetailEntries_DoesNotDuplicateEntriesWithBothDescriptorsAndJournal()
    {
        var today = DateTime.Today;
        var entries = new[] { Entry(today, descriptors: "calm") };
        var journalDates = new HashSet<DateTime> { today };

        var result = MoodCalendar.GetDetailEntries(entries, journalDates);

        await Assert.That(result.Count).IsEqualTo(1);
    }

    [Test]
    public async Task GetDetailEntries_ReturnsEmpty_WhenInputIsEmpty()
    {
        var result = MoodCalendar.GetDetailEntries([], journalDates: null);

        await Assert.That(result.Count).IsEqualTo(0);
    }

    [Test]
    public async Task GetDetailEntries_WithNullJournalDates_OnlyIncludesEntriesWithDescriptors()
    {
        var today = DateTime.Today;
        var entries = new[]
        {
            Entry(today, descriptors: "calm"),
            Entry(today.AddDays(-1), descriptors: ""),
        };

        var result = MoodCalendar.GetDetailEntries(entries, journalDates: null);

        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result[0].Descriptors).IsEqualTo("calm");
    }

    [Test]
    public async Task GetDetailEntries_IncludesAllMatchingEntries()
    {
        var today = DateTime.Today;
        var entries = new[]
        {
            Entry(today, descriptors: "calm"),
            Entry(today.AddDays(-1), descriptors: ""),
            Entry(today.AddDays(-2), descriptors: "focused"),
        };
        var journalDates = new HashSet<DateTime> { today.AddDays(-1) };

        var result = MoodCalendar.GetDetailEntries(entries, journalDates);

        await Assert.That(result.Count).IsEqualTo(3);
    }
}