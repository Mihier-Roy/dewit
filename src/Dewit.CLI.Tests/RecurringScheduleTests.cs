using Dewit.Core.Entities;

namespace Dewit.CLI.Tests;

public class RecurringScheduleTests
{
    // --- ComputeNextDueDate: base date is today ---

    [Test]
    public async Task ComputeNextDueDate_Daily_AddsOneDay()
    {
        var schedule = new RecurringSchedule { FrequencyType = "daily", Interval = 1 };
        var result = schedule.ComputeNextDueDate(DateTime.Today);
        await Assert.That(result).IsEqualTo(DateTime.Today.AddDays(1));
    }

    [Test]
    public async Task ComputeNextDueDate_Weekly_AddsSevenDays()
    {
        var schedule = new RecurringSchedule { FrequencyType = "weekly", Interval = 1 };
        var result = schedule.ComputeNextDueDate(DateTime.Today);
        await Assert.That(result).IsEqualTo(DateTime.Today.AddDays(7));
    }

    [Test]
    public async Task ComputeNextDueDate_Monthly_AddsOneMonth()
    {
        var schedule = new RecurringSchedule { FrequencyType = "monthly", Interval = 1 };
        var baseDate = new DateTime(2026, 1, 15);
        var result = schedule.ComputeNextDueDate(baseDate);
        await Assert.That(result).IsEqualTo(new DateTime(2026, 2, 15));
    }

    [Test]
    public async Task ComputeNextDueDate_Monthly_ClampsToMonthEnd()
    {
        var schedule = new RecurringSchedule { FrequencyType = "monthly", Interval = 1 };
        var baseDate = new DateTime(2026, 1, 31);
        var result = schedule.ComputeNextDueDate(baseDate);
        await Assert.That(result).IsEqualTo(new DateTime(2026, 2, 28));
    }

    [Test]
    public async Task ComputeNextDueDate_Monthly_ClampsToLeapFebEnd()
    {
        var schedule = new RecurringSchedule { FrequencyType = "monthly", Interval = 1 };
        var baseDate = new DateTime(2024, 1, 31);
        var result = schedule.ComputeNextDueDate(baseDate);
        await Assert.That(result).IsEqualTo(new DateTime(2024, 2, 29));
    }

    [Test]
    public async Task ComputeNextDueDate_Yearly_AddsOneYear()
    {
        var schedule = new RecurringSchedule { FrequencyType = "yearly", Interval = 1 };
        var baseDate = new DateTime(2025, 6, 15);
        var result = schedule.ComputeNextDueDate(baseDate);
        await Assert.That(result).IsEqualTo(new DateTime(2026, 6, 15));
    }

    [Test]
    public async Task ComputeNextDueDate_Yearly_ClampsLeapDay()
    {
        var schedule = new RecurringSchedule { FrequencyType = "yearly", Interval = 1 };
        var baseDate = new DateTime(2024, 2, 29);
        var result = schedule.ComputeNextDueDate(baseDate);
        await Assert.That(result).IsEqualTo(new DateTime(2025, 2, 28));
    }

    [Test]
    public async Task ComputeNextDueDate_Every3Days()
    {
        var schedule = new RecurringSchedule { FrequencyType = "daily", Interval = 3 };
        var result = schedule.ComputeNextDueDate(DateTime.Today);
        await Assert.That(result).IsEqualTo(DateTime.Today.AddDays(3));
    }

    [Test]
    public async Task ComputeNextDueDate_Every2Weeks()
    {
        var schedule = new RecurringSchedule { FrequencyType = "weekly", Interval = 2 };
        var result = schedule.ComputeNextDueDate(DateTime.Today);
        await Assert.That(result).IsEqualTo(DateTime.Today.AddDays(14));
    }

    // --- ComputeNextDueDate: pure computation, no clamping (clamping lives in TaskService) ---

    [Test]
    public async Task ComputeNextDueDate_PastBaseDate_ReturnsRelativeToPastDate()
    {
        var schedule = new RecurringSchedule { FrequencyType = "daily", Interval = 1 };
        var pastDate = DateTime.Today.AddDays(-10);
        var result = schedule.ComputeNextDueDate(pastDate);
        await Assert.That(result).IsEqualTo(pastDate.AddDays(1));
    }

    [Test]
    public async Task ComputeNextDueDate_WeeklyPastDate_ReturnsRelativeToPastDate()
    {
        var schedule = new RecurringSchedule { FrequencyType = "weekly", Interval = 1 };
        var pastDate = DateTime.Today.AddDays(-30);
        var result = schedule.ComputeNextDueDate(pastDate);
        await Assert.That(result).IsEqualTo(pastDate.AddDays(7));
    }

    // --- ComputeNextDueDate: unknown type throws ---

    [Test]
    public async Task ComputeNextDueDate_UnknownFrequencyType_ThrowsInvalidOperationException()
    {
        var schedule = new RecurringSchedule { FrequencyType = "hourly", Interval = 1 };
        await Assert.That(() => schedule.ComputeNextDueDate(DateTime.Today))
            .Throws<InvalidOperationException>();
    }

    // --- ToLabel ---

    [Test]
    [Arguments("daily", 1, "Daily")]
    [Arguments("weekly", 1, "Weekly")]
    [Arguments("monthly", 1, "Monthly")]
    [Arguments("yearly", 1, "Yearly")]
    [Arguments("daily", 2, "Every 2 Days")]
    [Arguments("weekly", 3, "Every 3 Weeks")]
    [Arguments("monthly", 2, "Every 2 Months")]
    [Arguments("yearly", 5, "Every 5 Years")]
    public async Task ToLabel_ReturnsCorrectLabel(
        string freqType,
        int interval,
        string expected
    )
    {
        var schedule = new RecurringSchedule { FrequencyType = freqType, Interval = interval };
        await Assert.That(schedule.ToLabel()).IsEqualTo(expected);
    }
}
