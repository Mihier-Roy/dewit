using Dewit.Core.Utils;

namespace Dewit.CLI.Tests.Utils;

public class RecurParserTests
{
    [Test]
    [Arguments("daily", "daily", 1)]
    [Arguments("weekly", "weekly", 1)]
    [Arguments("monthly", "monthly", 1)]
    [Arguments("yearly", "yearly", 1)]
    [Arguments("Daily", "daily", 1)]
    [Arguments("WEEKLY", "weekly", 1)]
    [Arguments("Monthly", "monthly", 1)]
    public async Task Parse_NamedKeyword_ReturnsCorrectSchedule(
        string input,
        string expectedType,
        int expectedInterval
    )
    {
        var result = RecurParser.Parse(input);
        await Assert.That(result.FrequencyType).IsEqualTo(expectedType);
        await Assert.That(result.Interval).IsEqualTo(expectedInterval);
    }

    [Test]
    [Arguments("2d", "daily", 2)]
    [Arguments("3w", "weekly", 3)]
    [Arguments("2m", "monthly", 2)]
    [Arguments("1y", "yearly", 1)]
    [Arguments("10d", "daily", 10)]
    [Arguments("2D", "daily", 2)]
    [Arguments("3W", "weekly", 3)]
    public async Task Parse_Shorthand_ReturnsCorrectSchedule(
        string input,
        string expectedType,
        int expectedInterval
    )
    {
        var result = RecurParser.Parse(input);
        await Assert.That(result.FrequencyType).IsEqualTo(expectedType);
        await Assert.That(result.Interval).IsEqualTo(expectedInterval);
    }

    [Test]
    [Arguments("")]
    [Arguments("  ")]
    public async Task Parse_EmptyOrWhitespace_ThrowsArgumentException(string input)
    {
        await Assert.That(() => RecurParser.Parse(input)).Throws<ArgumentException>();
    }

    [Test]
    public async Task Parse_Null_ThrowsArgumentException()
    {
        await Assert.That(() => RecurParser.Parse(null!)).Throws<ArgumentException>();
    }

    [Test]
    [Arguments("0d")]
    [Arguments("0w")]
    [Arguments("0m")]
    public async Task Parse_ZeroInterval_ThrowsArgumentException(string input)
    {
        await Assert.That(() => RecurParser.Parse(input)).Throws<ArgumentException>();
    }

    [Test]
    [Arguments("hourly")]
    [Arguments("biweekly")]
    [Arguments("fortnightly")]
    [Arguments("3x")]
    [Arguments("abc")]
    [Arguments("d2")]
    public async Task Parse_InvalidInput_ThrowsArgumentException(string input)
    {
        await Assert.That(() => RecurParser.Parse(input)).Throws<ArgumentException>();
    }

    [Test]
    public async Task TryParse_ValidInput_ReturnsTrueAndSchedule()
    {
        var success = RecurParser.TryParse("daily", out var schedule);
        await Assert.That(success).IsTrue();
        await Assert.That(schedule).IsNotNull();
        await Assert.That(schedule!.FrequencyType).IsEqualTo("daily");
    }

    [Test]
    public async Task TryParse_InvalidInput_ReturnsFalseAndNull()
    {
        var success = RecurParser.TryParse("hourly", out var schedule);
        await Assert.That(success).IsFalse();
        await Assert.That(schedule).IsNull();
    }

    [Test]
    public async Task TryParse_EmptyInput_ReturnsFalseAndNull()
    {
        var success = RecurParser.TryParse("", out var schedule);
        await Assert.That(success).IsFalse();
        await Assert.That(schedule).IsNull();
    }
}
