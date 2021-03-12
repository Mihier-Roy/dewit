using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Dewit.CLI.Models;

namespace Dewit.CLI.Helpers
{
	public class CsvContext
	{
		public static IEnumerable<TaskItem> ReadCSV(string path)
		{
			// Open the file at the specified path and return an IEnumerable of TaskItems
			using (var reader = new StreamReader(path, Encoding.UTF8))
			{
				using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				{
					return csv.GetRecords<TaskItem>().ToList();
				}
			}
		}

		public static void WriteCSV(string path, TaskItem task)
		{
			// Create an IEnumerable object with all the task records to be written.
			var taskRecords = new List<TaskItem>(){
				task
			};

			// If the file does not exist, create the file.
			// Else, append to the file and do not re-write the header.
			if (!File.Exists(path))
			{
				using (var writer = new StreamWriter(path))
				{
					using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
					{
						csv.WriteRecords(taskRecords);
					}
				}
			}
			else
			{
				var config = new CsvConfiguration(CultureInfo.InvariantCulture)
				{
					HasHeaderRecord = false,
				};
				using (var stream = File.Open(path, FileMode.Append))
				{
					using (var writer = new StreamWriter(stream))
					{
						using (var csv = new CsvWriter(writer, config))
						{
							csv.WriteRecords(taskRecords);
						}
					}
				}
			}
		}
	}
}

