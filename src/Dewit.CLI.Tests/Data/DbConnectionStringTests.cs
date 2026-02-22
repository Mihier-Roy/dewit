using Dewit.Data.Data;

namespace Dewit.CLI.Tests.Data;

[NotInParallel("DEWIT_DIR")]
public class DbConnectionStringTests
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
    public async Task Get_ReturnsSqliteConnectionString()
    {
        var result = DbConnectionString.Get();
        await Assert.That(result).StartsWith("Data Source=");
    }

    [Test]
    public async Task Get_PointsToDewitDb()
    {
        var result = DbConnectionString.Get();
        await Assert.That(result).Contains("dewit.db");
    }

    [Test]
    public async Task Get_PathIsUnderUserProfileDewit()
    {
        var result = DbConnectionString.Get();
        var expectedDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".dewit"
        );
        await Assert.That(result).Contains(expectedDir);
    }

    [Test]
    public async Task Get_CreatesDirectoryIfMissing()
    {
        DbConnectionString.Get();

        var expectedDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".dewit"
        );
        await Assert.That(Directory.Exists(expectedDir)).IsTrue();
    }

    [Test]
    public async Task Get_UsesDewitDirEnvVarIfSet()
    {
        var customDir = Path.Combine(Path.GetTempPath(), $"dewit_test_{Guid.NewGuid()}");
        Environment.SetEnvironmentVariable("DEWIT_DIR", customDir);

        var result = DbConnectionString.Get();

        await Assert.That(result).Contains(customDir);

        // Cleanup
        if (Directory.Exists(customDir))
            Directory.Delete(customDir, recursive: true);
    }
}