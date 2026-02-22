namespace Dewit.Core.Interfaces
{
    public interface IConfigurationService
    {
        string? GetValue(string key);
        void SetValue(string key, string value);
        void DeleteValue(string key);
        bool KeyExists(string key);
        IEnumerable<KeyValuePair<string, string>> GetAll();
    }
}