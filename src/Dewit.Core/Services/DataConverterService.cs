using System.Globalization;
using System.Text.Json;
using CsvHelper;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dewit.Core.Services
{
    public class DataConverterService : IDataConverter
    {
        private readonly ILogger<DataConverterService> _logger;

        public DataConverterService(ILogger<DataConverterService> logger)
        {
            _logger = logger;
        }

        public void ExportToFile<T>(IEnumerable<T> data, string filePath, DataFormats format)
        {
            _logger.LogInformation(
                "Exporting data to {FilePath} in {Format} format",
                filePath,
                format
            );

            try
            {
                switch (format)
                {
                    case DataFormats.Json:
                        ExportToJson(data, filePath);
                        break;
                    case DataFormats.Csv:
                        ExportToCsv(data, filePath);
                        break;
                    default:
                        throw new ArgumentException(
                            $"Unsupported format: {format}",
                            nameof(format)
                        );
                }

                _logger.LogInformation("Successfully exported data to {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export data to {FilePath}", filePath);
                throw new ApplicationException($"Failed to export data to {filePath}", ex);
            }
        }

        public IEnumerable<T> ImportFromFile<T>(string filePath, DataFormats format)
        {
            _logger.LogInformation(
                "Importing data from {FilePath} in {Format} format",
                filePath,
                format
            );

            if (!File.Exists(filePath))
            {
                _logger.LogError("File not found: {FilePath}", filePath);
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            try
            {
                IEnumerable<T> data = format switch
                {
                    DataFormats.Json => ImportFromJson<T>(filePath),
                    DataFormats.Csv => ImportFromCsv<T>(filePath),
                    _ => throw new ArgumentException(
                        $"Unsupported format: {format}",
                        nameof(format)
                    ),
                };

                _logger.LogInformation("Successfully imported data from {FilePath}", filePath);
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import data from {FilePath}", filePath);
                throw new ApplicationException($"Failed to import data from {filePath}", ex);
            }
        }

        private void ExportToJson<T>(IEnumerable<T> data, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
        }

        private IEnumerable<T> ImportFromJson<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var data = JsonSerializer.Deserialize<IEnumerable<T>>(json, options);
            return data ?? Enumerable.Empty<T>();
        }

        private void ExportToCsv<T>(IEnumerable<T> data, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(data);
        }

        private IEnumerable<T> ImportFromCsv<T>(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<T>().ToList();
            return records;
        }
    }
}
