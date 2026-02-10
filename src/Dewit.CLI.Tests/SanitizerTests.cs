using Dewit.CLI.Utils;

namespace Dewit.CLI.Tests;

public class SanitizerTests
{
    [Test]
    [Arguments("work,testing", "work,testing")]
    [Arguments("work, testing", "work,testing")]
    [Arguments("work!@#testing", "worktesting")]
    [Arguments("work_test,code", "work_test,code")]
    [Arguments("UPPER,lower", "UPPER,lower")]
    [Arguments("tag1,tag2,", "tag1,tag2")]
    public async Task SanitizeTags_RemovesInvalidCharacters(string input, string expected)
    {
        var result = Sanitizer.SanitizeTags(input);
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task SanitizeTags_AllowsAlphanumericUnderscoreAndComma()
    {
        var result = Sanitizer.SanitizeTags("abc,123,test_tag");
        await Assert.That(result).IsEqualTo("abc,123,test_tag");
    }

    [Test]
    public async Task SanitizeTags_RemovesSpaces()
    {
        var result = Sanitizer.SanitizeTags("tag one, tag two");
        await Assert.That(result).IsEqualTo("tagone,tagtwo");
    }

    [Test]
    public async Task SanitizeTags_RemovesSpecialCharacters()
    {
        var result = Sanitizer.SanitizeTags("hello!@#$%^&*()world");
        await Assert.That(result).IsEqualTo("helloworld");
    }

    [Test]
    public async Task SanitizeTags_RemovesTrailingComma()
    {
        var result = Sanitizer.SanitizeTags("tag1,tag2,");
        await Assert.That(result).IsEqualTo("tag1,tag2");
    }

    [Test]
    [Arguments("work,work", "work")]
    [Arguments("a,b,a,c,b", "a,b,c")]
    [Arguments("single", "single")]
    public async Task DeduplicateTags_RemovesDuplicates(string input, string expected)
    {
        var result = Sanitizer.DeduplicateTags(input);
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task DeduplicateTags_PreservesUniqueTagsOrder()
    {
        var result = Sanitizer.DeduplicateTags("alpha,beta,gamma");
        await Assert.That(result).IsEqualTo("alpha,beta,gamma");
    }

    [Test]
    public async Task DeduplicateTags_HandlesSingleTag()
    {
        var result = Sanitizer.DeduplicateTags("onlytag");
        await Assert.That(result).IsEqualTo("onlytag");
    }

    [Test]
    public async Task SanitizeAndDeduplicate_WorkTogether()
    {
        var sanitized = Sanitizer.SanitizeTags("work, work, test!");
        var deduped = Sanitizer.DeduplicateTags(sanitized);
        await Assert.That(deduped).IsEqualTo("work,test");
    }
}
