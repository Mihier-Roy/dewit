using System;
using System.Diagnostics;

namespace Dewit.CLI.Utils
{
    public static class EditorHelper
    {
        public static void Open(string filePath)
        {
            var editor =
                Environment.GetEnvironmentVariable("EDITOR")
                ?? Environment.GetEnvironmentVariable("VISUAL");

            ProcessStartInfo psi =
                editor != null
                    ? new ProcessStartInfo
                    {
                        FileName = editor,
                        Arguments = $"\"{filePath}\"",
                        UseShellExecute = false,
                    }
                    : new ProcessStartInfo(filePath) { UseShellExecute = true };

            Process.Start(psi)?.WaitForExit();
        }
    }
}
