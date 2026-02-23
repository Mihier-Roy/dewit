namespace Dewit.Core.Utils
{
    public static class DewitDirectory
    {
        public static string GetBaseDir()
        {
            var env = Environment.GetEnvironmentVariable("DEWIT_DIR");
            return !string.IsNullOrEmpty(env)
                ? env
                : Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".dewit"
                );
        }

        public static void EnsureExists() => Directory.CreateDirectory(GetBaseDir());
    }
}
