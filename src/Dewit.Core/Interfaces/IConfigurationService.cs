using Dewit.Core.Entities;

namespace Dewit.Core.Interfaces;

public interface IConfigurationService
{
    string GetValue(int key);
    void SetValue(int key, string value);
    IEnumerable<ConfigItem> ListValues();
}