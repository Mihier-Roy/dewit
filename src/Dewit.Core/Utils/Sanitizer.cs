using System.Text.RegularExpressions;

namespace Dewit.Core.Utils
{
    public static class Sanitizer
    {
        public static string SanitizeTags(string input)
        {
            Regex r = new(
                "(?:[^a-z0-9,_])",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled
            );
            var output = r.Replace(input, string.Empty).ToLower();
            return output[^1] == ',' ? output.Remove(output.Length - 1) : output;
        }

        public static string DeduplicateTags(string input)
        {
            string[] tags = input.Split(',');
            var hashSet = new HashSet<string>(tags);
            return string.Join(',', hashSet.ToArray());
        }
    }
}
