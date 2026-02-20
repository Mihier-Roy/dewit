using Dewit.Core.Enums;

namespace Dewit.CLI.Tests.Enums;

public class MoodExtensionsTests
{
    // ToDisplayName

    [Test]
    [Arguments(Mood.VeryHappy, "Very Happy")]
    [Arguments(Mood.Happy, "Happy")]
    [Arguments(Mood.Meh, "Meh")]
    [Arguments(Mood.Down, "Down")]
    [Arguments(Mood.ExtraDown, "Extra Down")]
    public async Task ToDisplayName_ReturnsCorrectLabel(Mood mood, string expected)
    {
        await Assert.That(mood.ToDisplayName()).IsEqualTo(expected);
    }

    // ToSpectreColor

    [Test]
    [Arguments(Mood.VeryHappy, "green")]
    [Arguments(Mood.Happy, "chartreuse2")]
    [Arguments(Mood.Meh, "yellow")]
    [Arguments(Mood.Down, "darkorange")]
    [Arguments(Mood.ExtraDown, "red")]
    public async Task ToSpectreColor_ReturnsCorrectColor(Mood mood, string expected)
    {
        await Assert.That(mood.ToSpectreColor()).IsEqualTo(expected);
    }

    [Test]
    public async Task ToSpectreColor_UnknownMood_ReturnsWhite()
    {
        var unknown = (Mood)99;
        await Assert.That(unknown.ToSpectreColor()).IsEqualTo("white");
    }

    // ToConfigKey

    [Test]
    [Arguments(Mood.VeryHappy, "mood.descriptors.veryhappy")]
    [Arguments(Mood.Happy, "mood.descriptors.happy")]
    [Arguments(Mood.Meh, "mood.descriptors.meh")]
    [Arguments(Mood.Down, "mood.descriptors.down")]
    [Arguments(Mood.ExtraDown, "mood.descriptors.extradown")]
    public async Task ToConfigKey_ReturnsCorrectKey(Mood mood, string expected)
    {
        await Assert.That(mood.ToConfigKey()).IsEqualTo(expected);
    }

    [Test]
    public async Task ToConfigKey_UnknownMood_ThrowsArgumentOutOfRangeException()
    {
        var unknown = (Mood)99;
        await Assert.That(() => unknown.ToConfigKey()).Throws<ArgumentOutOfRangeException>();
    }

    // TryParse

    [Test]
    [Arguments("veryhappy", Mood.VeryHappy)]
    [Arguments("very happy", Mood.VeryHappy)]
    [Arguments("happy", Mood.Happy)]
    [Arguments("meh", Mood.Meh)]
    [Arguments("down", Mood.Down)]
    [Arguments("extradown", Mood.ExtraDown)]
    [Arguments("extra down", Mood.ExtraDown)]
    public async Task TryParse_ValidInput_ReturnsTrueAndCorrectMood(string input, Mood expected)
    {
        var result = MoodExtensions.TryParse(input, out var mood);

        await Assert.That(result).IsTrue();
        await Assert.That(mood).IsEqualTo(expected);
    }

    [Test]
    [Arguments("HAPPY")]
    [Arguments("VeryHappy")]
    [Arguments("Extra Down")]
    public async Task TryParse_ValidInput_IsCaseInsensitive(string input)
    {
        var result = MoodExtensions.TryParse(input, out _);
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("excellent")]
    [Arguments("")]
    [Arguments("ok")]
    public async Task TryParse_InvalidInput_ReturnsFalse(string input)
    {
        var result = MoodExtensions.TryParse(input, out _);
        await Assert.That(result).IsFalse();
    }
}
