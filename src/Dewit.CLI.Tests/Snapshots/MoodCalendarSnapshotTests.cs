using Dewit.CLI.Utils;
using Dewit.Core.Entities;
using Spectre.Console;
using Spectre.Console.Testing;

namespace Dewit.CLI.Tests.Snapshots;

public class MoodCalendarSnapshotTests
{
    // 2024-06-03 is a Monday — fixed anchor for all week-based tests
    private static readonly DateTime FixedMonday = new(2024, 6, 3);

    private static string Render(Action<IAnsiConsole> renderAction)
    {
        var console = new TestConsole();
        console.Profile.Width = 120;
        renderAction(console);
        return console.Output;
    }

    private static MoodEntry Entry(DateTime date, string mood, string descriptors = "") =>
        new()
        {
            Date = date.Date,
            Mood = mood,
            Descriptors = descriptors,
        };

    private static MoodEntry[] SampleWeekEntries() =>
        [
            Entry(FixedMonday, "Happy", "calm,focused"),
            Entry(FixedMonday.AddDays(1), "VeryHappy"),
            Entry(FixedMonday.AddDays(3), "Meh", "tired"),
            Entry(FixedMonday.AddDays(5), "Down"),
        ];

    private static MoodEntry[] SampleMonthEntries() =>
        [
            Entry(new DateTime(2024, 6, 3), "Happy", "calm"),
            Entry(new DateTime(2024, 6, 5), "VeryHappy"),
            Entry(new DateTime(2024, 6, 10), "Meh", "tired"),
            Entry(new DateTime(2024, 6, 17), "Down", "anxious"),
            Entry(new DateTime(2024, 6, 24), "Happy"),
        ];

    // ── Week ──────────────────────────────────────────────────────────────────

    [Test]
    public Task RenderWeek_Empty() =>
        Verify(Render(c => MoodCalendar.RenderWeek(c, FixedMonday, [])));

    [Test]
    public Task RenderWeek_WithMoods() =>
        Verify(Render(c => MoodCalendar.RenderWeek(c, FixedMonday, SampleWeekEntries())));

    [Test]
    public Task RenderWeek_WithJournalIndicators() =>
        Verify(
            Render(c =>
                MoodCalendar.RenderWeek(
                    c,
                    FixedMonday,
                    SampleWeekEntries(),
                    journalDates: [FixedMonday, FixedMonday.AddDays(2)]
                )
            )
        );

    [Test]
    public Task RenderWeek_InNavMode() =>
        Verify(
            Render(c =>
                MoodCalendar.RenderWeek(
                    c,
                    FixedMonday,
                    SampleWeekEntries(),
                    selectedDescriptorIndex: 0
                )
            )
        );

    // ── Month ─────────────────────────────────────────────────────────────────

    [Test]
    public Task RenderMonth_Empty() =>
        Verify(Render(c => MoodCalendar.RenderMonth(c, 2024, 6, [])));

    [Test]
    public Task RenderMonth_WithMoods() =>
        Verify(Render(c => MoodCalendar.RenderMonth(c, 2024, 6, SampleMonthEntries())));

    [Test]
    public Task RenderMonth_InNavMode() =>
        Verify(
            Render(c =>
                MoodCalendar.RenderMonth(
                    c,
                    2024,
                    6,
                    SampleMonthEntries(),
                    selectedDescriptorIndex: 1
                )
            )
        );

    // ── Quarter ───────────────────────────────────────────────────────────────

    [Test]
    public Task RenderQuarter_WithMoods() =>
        Verify(Render(c => MoodCalendar.RenderQuarter(c, 2024, 2, SampleMonthEntries())));

    // ── Year ──────────────────────────────────────────────────────────────────

    [Test]
    public Task RenderYear_WithMoods() =>
        Verify(Render(c => MoodCalendar.RenderYear(c, 2024, SampleMonthEntries())));
}
