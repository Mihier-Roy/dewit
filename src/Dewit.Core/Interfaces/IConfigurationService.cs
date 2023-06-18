namespace Dewit.Core.Interfaces;

public interface IConfigurationService
{
    string GetValue(int key);
    void SetValue(int key, string value);
}