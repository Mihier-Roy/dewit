using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public static IEnumerable<TaskItem> FromType(string path, string type)
        {
            if (type == "csv")
            {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    return csv.GetRecords<TaskItem>().ToList();
                }
            }
            else if (type == "json")
            {
                var jsonString = File.ReadAllText(path);
                return JsonSerializer.Deserialize<IEnumerable<TaskItem>>(jsonString);
            }
            else
            {
                throw new Exception("Invalid type");
            }
        }
    }
}
