using Dewit.Core.Utils;

namespace Dewit.CLI.Tests.Utils;

public class DateParserTests
{
    [Test]
    public async Task Parse_Today_ReturnsToday()
    {
        var result = DateParser.Parse("today");
        await Assert.That(result.Date).IsEqualTo(DateTime.Today);
    }

    [Test]
    public async Task Parse_Yesterday_ReturnsYesterday()
    {
        var result = DateParser.Parse("yesterday");
        await Assert.That(result.Date).IsEqualTo(DateTime.Today.AddDays(-1));
    }

    [Test]
    public async Task Parse_LastMonday_ReturnsPastMonday()
    {
        var result = DateParser.Parse("last monday");
        await Assert.That(result.DayOfWeek).IsEqualTo(DayOfWeek.Monday);
        await Assert.That(result.Date).IsLessThan(DateTime.Today.AddDays(1));
    }

    [Test]
    public async Task Parse_FullDate_ParsesCorrectly()
    {
        var result = DateParser.Parse("2026-01-15");
        await Assert.That(result.Year).IsEqualTo(2026);
        await Assert.That(result.Month).IsEqualTo(1);
        await Assert.That(result.Day).IsEqualTo(15);
    }

    [Test]
    public async Task Parse_MonthDay_AssumesCurrentYear()
    {
        var result = DateParser.Parse("01-15");
        await Assert.That(result.Year).IsEqualTo(DateTime.Today.Year);
        await Assert.That(result.Month).IsEqualTo(1);
        await Assert.That(result.Day).IsEqualTo(15);
    }

    [Test]
    public async Task Parse_FutureDate_ThrowsArgumentException()
    {
        var future = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
        await Assert.That(() => DateParser.Parse(future)).Throws<ArgumentException>();
    }

    [Test]
    public async Task Parse_InvalidString_ThrowsArgumentException()
    {
        await Assert.That(() => DateParser.Parse("not-a-date")).Throws<ArgumentException>();
    }
}
