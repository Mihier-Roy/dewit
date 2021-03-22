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
		public static void ToCsv(IEnumerable<TaskItem> tasks, string path)
		{
			using (var writer = new StreamWriter(path))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteRecords(tasks);
			}
		}
	}
}