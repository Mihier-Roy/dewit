namespace Dewit.Data.Data
{
    public static class DbConnectionString
    {
        public static string Get()
        {
            var configDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config",
                "dewit"
            );

            Directory.CreateDirectory(configDir);

            return $"Data Source={Path.Combine(configDir, "dewit_tasks.db")}";
        }
    }
}
