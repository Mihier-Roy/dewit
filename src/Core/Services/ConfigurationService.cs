using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dewit.Core.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IRepository<ConfigItem> _configRepository;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IRepository<ConfigItem> configRepository, ILogger<ConfigurationService> logger)
    {
        _configRepository = configRepository;
        _logger = logger;
    }

    public string GetValue(int key)
    {
        _logger.LogDebug("Getting configuration item: {Key}", key);
        var item = _configRepository.GetById(key);
        return item != null ? item.Value : null;
    }

    public void SetValue(int key, string value)
    {
        _logger.LogDebug("Setting configuration item: {Key}, {Value}", key, value);
        var item = _configRepository.GetById(key);

        if (item != null)
        {
            _logger.LogDebug("Configuration item exists. Updating: {Key}, {Value}", key, value);
            item.Value = value;
            _configRepository.Update(item);
        }
        else
        {
            _logger.LogDebug("Configuration item does not exist. Creating: {Key}, {Value}", key, value);
            _configRepository.Add(new ConfigItem() { Value = value });
        }
    }
}