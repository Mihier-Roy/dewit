using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dewit.Core.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IRepository<ConfigItem> _repository;
        private readonly ILogger<ConfigurationService> _logger;

        public ConfigurationService(
            IRepository<ConfigItem> repository,
            ILogger<ConfigurationService> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public string? GetValue(string key)
        {
            _logger.LogDebug("Getting configuration value for key: {Key}", key);

            var configItem = _repository
                .List()
                .FirstOrDefault(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            return configItem?.Value;
        }

        public void SetValue(string key, string value)
        {
            _logger.LogInformation("Setting configuration value for key: {Key}", key);

            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogError("Configuration key cannot be empty");
                throw new ArgumentException("Configuration key cannot be empty", nameof(key));
            }

            var existingItem = _repository
                .List()
                .FirstOrDefault(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (existingItem != null)
            {
                // Update existing
                existingItem.Value = value;
                existingItem.UpdatedAt = DateTime.Now;
                _repository.Update(existingItem);
                _logger.LogInformation("Updated configuration: {Key}", key);
            }
            else
            {
                // Create new
                var newItem = new ConfigItem
                {
                    Key = key,
                    Value = value,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                _repository.Add(newItem);
                _logger.LogInformation("Created new configuration: {Key}", key);
            }
        }

        public void DeleteValue(string key)
        {
            _logger.LogInformation("Deleting configuration value for key: {Key}", key);

            var configItem = _repository
                .List()
                .FirstOrDefault(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (configItem != null)
            {
                _repository.Remove(configItem);
                _logger.LogInformation("Deleted configuration: {Key}", key);
            }
            else
            {
                _logger.LogWarning("Configuration key not found: {Key}", key);
            }
        }

        public bool KeyExists(string key)
        {
            return _repository
                .List()
                .Any(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<KeyValuePair<string, string>> GetAll()
        {
            _logger.LogDebug("Getting all configuration values");

            return _repository
                .List()
                .Select(c => new KeyValuePair<string, string>(c.Key, c.Value))
                .ToList();
        }
    }
}
