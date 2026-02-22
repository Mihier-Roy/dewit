using Dewit.Core.Enums;

namespace Dewit.Core.Interfaces
{
    public interface IDataConverter
    {
        void ExportToFile<T>(IEnumerable<T> data, string filePath, DataFormats format);
        IEnumerable<T> ImportFromFile<T>(string filePath, DataFormats format);
    }
}