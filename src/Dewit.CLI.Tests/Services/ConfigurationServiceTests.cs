using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Dewit.Core.Services;
using Dewit.Data.Data;
using Dewit.Data.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dewit.CLI.Tests.Services;

public class ConfigurationServiceTests
{
    private DewitDbContext _context = null!;
    private IRepository<ConfigItem> _repository = null!;
    private IConfigurationService _service = null!;
    private ILogger<ConfigurationService> _logger = null!;

    [Before(Test)]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DewitDbContext>()
            .UseInMemoryDatabase($"Test_{Guid.NewGuid()}")
            .Options;
        _context = new DewitDbContext(options);
        _repository = new Repository<ConfigItem>(_context);
        _logger = LoggerFactory.Create(b => { }).CreateLogger<ConfigurationService>();
        _service = new ConfigurationService(_repository, _logger);
    }

    [After(Test)]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [Test]
    public async Task SetValue_CreatesNewConfig()
    {
        _service.SetValue("TestKey", "TestValue");

        var value = _service.GetValue("TestKey");
        await Assert.That(value).IsEqualTo("TestValue");
    }

    [Test]
    public async Task SetValue_UpdatesExistingConfig()
    {
        _service.SetValue("TestKey", "InitialValue");
        _service.SetValue("TestKey", "UpdatedValue");

        var value = _service.GetValue("TestKey");
        await Assert.That(value).IsEqualTo("UpdatedValue");
    }

    [Test]
    public async Task GetValue_ReturnsNullForNonExistentKey()
    {
        var value = _service.GetValue("NonExistent");

        await Assert.That(value).IsNull();
    }

    [Test]
    public async Task KeyExists_ReturnsTrueForExistingKey()
    {
        _service.SetValue("TestKey", "TestValue");

        var exists = _service.KeyExists("TestKey");

        await Assert.That(exists).IsTrue();
    }

    [Test]
    public async Task KeyExists_ReturnsFalseForNonExistentKey()
    {
        var exists = _service.KeyExists("NonExistent");

        await Assert.That(exists).IsFalse();
    }

    [Test]
    public async Task DeleteValue_RemovesConfig()
    {
        _service.SetValue("TestKey", "TestValue");
        _service.DeleteValue("TestKey");

        var value = _service.GetValue("TestKey");
        await Assert.That(value).IsNull();
    }

    [Test]
    public async Task DeleteValue_DoesNotThrowForNonExistentKey()
    {
        _service.DeleteValue("NonExistent");

        // Should not throw
        await Assert.That(true).IsTrue();
    }

    [Test]
    public async Task GetAll_ReturnsAllConfigurations()
    {
        _service.SetValue("Key1", "Value1");
        _service.SetValue("Key2", "Value2");
        _service.SetValue("Key3", "Value3");

        var all = _service.GetAll().ToList();

        await Assert.That(all.Count).IsEqualTo(3);
        await Assert.That(all.Any(kv => kv.Key == "Key1" && kv.Value == "Value1")).IsTrue();
    }

    [Test]
    public async Task SetValue_ThrowsForEmptyKey()
    {
        await Assert.That(() => _service.SetValue("", "value")).Throws<ArgumentException>();
    }

    [Test]
    public async Task KeyExists_IsCaseInsensitive()
    {
        _service.SetValue("TestKey", "Value");

        var exists1 = _service.KeyExists("testkey");
        var exists2 = _service.KeyExists("TESTKEY");

        await Assert.That(exists1).IsTrue();
        await Assert.That(exists2).IsTrue();
    }
}
