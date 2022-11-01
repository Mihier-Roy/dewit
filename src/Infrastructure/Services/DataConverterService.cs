using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using System.Globalization;
using System.Text.Json;
using CsvHelper;

namespace Dewit.Core.Services
{
	public class DataConverterService<T> : IDataConverter<T>
	{
		public void ToFormat(DataFormats type, string path, IEnumerable<T> items)
		{
			if (type == DataFormats.Csv)
			{
				using (var writer = new StreamWriter(path))
				using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
				{
					csv.WriteRecords(items);
				}
			}
			else if (type == DataFormats.Json)
			{
				string json = JsonSerializer.Serialize(items);
				File.WriteAllText(path, json);
			}
		}

		public IEnumerable<T> FromFormat(DataFormats type, string path)
		{
			if (type == DataFormats.Csv)
			{
				using (var reader = new StreamReader(path))
				using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				{
					return csv.GetRecords<T>().ToList();
				}
			}
			else if (type == DataFormats.Json)
			{
				var jsonString = File.ReadAllText(path);
				return JsonSerializer.Deserialize<IEnumerable<T>>(jsonString) ?? new List<T>();
			}
			else
			{
				throw new Exception("Invalid type");
			}
		}
	}
}