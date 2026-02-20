using Dewit.Data.Data;

namespace Dewit.CLI.Tests.Data;

public class DbConnectionStringTests
{
    [Test]
    public async Task Get_ReturnsSqliteConnectionString()
    {
        var result = DbConnectionString.Get();
        await Assert.That(result).StartsWith("Data Source=");
    }

    [Test]
    public async Task Get_PointsToDewittaskesDb()
    {
        var result = DbConnectionString.Get();
        await Assert.That(result).Contains("dewit_tasks.db");
    }

    [Test]
    public async Task Get_PathIsUnderUserProfile()
    {
        var result = DbConnectionString.Get();
        var expectedDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config",
            "dewit"
        );
        await Assert.That(result).Contains(expectedDir);
    }

    [Test]
    public async Task Get_CreatesDirectoryIfMissing()
    {
        DbConnectionString.Get();

        var expectedDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config",
            "dewit"
        );
        await Assert.That(Directory.Exists(expectedDir)).IsTrue();
    }
}
