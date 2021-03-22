using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using CsvHelper;
using Dewit.CLI.Models;

namespace Dewit.CLI.Utils
{
	public static class FormatData
	{
		public static void ToType(IEnumerable<TaskItem> tasks, string path, string type)
		{
			if (type == "csv")
			{

				using (var writer = new StreamWriter(path))
				using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
				{
					csv.WriteRecords(tasks);
				}
			}
			else if (type == "json")
			{
				string json = JsonSerializer.Serialize(tasks);
				File.WriteAllText(path, json);
			}
		}
	}
}