using System.Text.RegularExpressions;

namespace Dewit.CLI.Utils
{
	public static class Sanitizer
	{
		public static string SanitizeTags(string input)
		{
			Regex r = new Regex("(?:[^a-z0-9,_])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
			var output = r.Replace(input, string.Empty);
			return output[output.Length - 1] == ',' ? output.Remove(output.Length - 1) : output;
		}
	}
}