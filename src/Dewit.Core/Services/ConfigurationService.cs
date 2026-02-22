using Dewit.Core.Entities;
using Dewit.Core.Interfaces;

namespace Dewit.Core.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IRepository<ConfigItem> _repository;

        public ConfigurationService(IRepository<ConfigItem> repository)
        {
            _repository = repository;
        }

        public string? GetValue(string key)
        {
            var configItem = _repository
                .List()
                .FirstOrDefault(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            return configItem?.Value;
        }

        public void SetValue(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Configuration key cannot be empty", nameof(key));

            var existingItem = _repository
                .List()
                .FirstOrDefault(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (existingItem != null)
            {
                existingItem.Value = value;
                existingItem.UpdatedAt = DateTime.Now;
                _repository.Update(existingItem);
            }
            else
            {
                var newItem = new ConfigItem
                {
                    Key = key,
                    Value = value,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                _repository.Add(newItem);
            }
        }

        public void DeleteValue(string key)
        {
            var configItem = _repository
                .List()
                .FirstOrDefault(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (configItem != null)
                _repository.Remove(configItem);
        }

        public bool KeyExists(string key)
        {
            return _repository
                .List()
                .Any(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<KeyValuePair<string, string>> GetAll()
        {
            return _repository
                .List()
                .Select(c => new KeyValuePair<string, string>(c.Key, c.Value))
                .ToList();
        }
    }
}