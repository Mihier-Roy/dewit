using Dewit.Core.Utils;

namespace Dewit.Data.Data
{
    public static class DbConnectionString
    {
        public static string Get()
        {
            var baseDir = DewitDirectory.GetBaseDir();
            Directory.CreateDirectory(baseDir);

            var newDbPath = Path.Combine(baseDir, "dewit.db");

            // One-time migration from old location
            var oldDbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config",
                "dewit",
                "dewit_tasks.db"
            );

            if (!File.Exists(newDbPath) && File.Exists(oldDbPath))
            {
                try
                {
                    File.Copy(oldDbPath, newDbPath);
                }
                catch (IOException)
                { /* Concurrent startup or file already appeared */
                }
            }

            return $"Data Source={newDbPath}";
        }
    }
}