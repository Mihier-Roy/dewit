using Dewit.Core.Utils;

namespace Dewit.CLI.Tests.Utils;

[NotInParallel("DEWIT_DIR")]
public class DewitDirectoryTests
{
    private string? _savedEnv;

    [Before(Test)]
    public void SaveEnv()
    {
        _savedEnv = Environment.GetEnvironmentVariable("DEWIT_DIR");
        Environment.SetEnvironmentVariable("DEWIT_DIR", null);
    }

    [After(Test)]
    public void RestoreEnv()
    {
        Environment.SetEnvironmentVariable("DEWIT_DIR", _savedEnv);
    }

    [Test]
    public async Task GetBaseDir_DefaultsToUserProfileDewit()
    {
        var result = DewitDirectory.GetBaseDir();
        var expected = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".dewit"
        );

        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task GetBaseDir_ReturnsDewitDirEnvVarIfSet()
    {
        var customPath = Path.Combine(Path.GetTempPath(), $"dewit_test_{Guid.NewGuid()}");
        Environment.SetEnvironmentVariable("DEWIT_DIR", customPath);

        var result = DewitDirectory.GetBaseDir();

        await Assert.That(result).IsEqualTo(customPath);
    }

    [Test]
    public async Task GetBaseDir_IgnoresEmptyDewitDirEnvVar()
    {
        Environment.SetEnvironmentVariable("DEWIT_DIR", "");

        var result = DewitDirectory.GetBaseDir();
        var expected = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".dewit"
        );

        await Assert.That(result).IsEqualTo(expected);
    }
}