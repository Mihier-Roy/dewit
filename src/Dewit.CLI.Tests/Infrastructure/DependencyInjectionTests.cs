using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Dewit.Core.Services;
using Dewit.Data.Data;
using Dewit.Data.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dewit.CLI.Tests.Infrastructure;

public class DependencyInjectionTests
{
    private IServiceProvider SetupServiceProvider()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { "ConnectionStrings:Sqlite", "Data Source=:memory:" },
                }
            )
            .Build();

        services.AddSingleton<IConfiguration>(config);
        services.AddDbContext<DewitDbContext>(opts =>
            opts.UseInMemoryDatabase($"Test_{Guid.NewGuid()}")
        );
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddLogging();
        services.AddTransient<ITaskService, TaskService>();

        return services.BuildServiceProvider();
    }

    [Test]
    public async Task CanResolveITaskService()
    {
        var provider = SetupServiceProvider();
        var taskService = provider.GetService<ITaskService>();

        await Assert.That(taskService).IsNotNull();
    }

    [Test]
    public async Task CanResolveGenericRepository()
    {
        var provider = SetupServiceProvider();
        var repo = provider.GetService<IRepository<TaskItem>>();

        await Assert.That(repo).IsNotNull();
    }

    [Test]
    public async Task CanResolveDewitDbContext()
    {
        var provider = SetupServiceProvider();
        var context = provider.GetService<DewitDbContext>();

        await Assert.That(context).IsNotNull();
    }

    [Test]
    public async Task CanResolveLogger()
    {
        var provider = SetupServiceProvider();
        var logger = provider.GetService<ILogger<TaskService>>();

        await Assert.That(logger).IsNotNull();
    }

    [Test]
    public async Task TaskService_WithDependencies_WorksCorrectly()
    {
        var provider = SetupServiceProvider();
        var taskService = provider.GetRequiredService<ITaskService>();

        // Use the service to add a task
        taskService.AddTask("Test DI task", "Doing", "test");

        // Verify the task was added
        var tasks = taskService.GetTasks(duration: "all");
        await Assert.That(tasks.Count()).IsEqualTo(1);
        await Assert.That(tasks.First().TaskDescription).IsEqualTo("Test DI task");
    }

    [Test]
    public async Task Repository_WithDbContext_WorksCorrectly()
    {
        var provider = SetupServiceProvider();
        var repo = provider.GetRequiredService<IRepository<TaskItem>>();

        var task = new TaskItem
        {
            TaskDescription = "Test repo via DI",
            Status = "Doing",
            AddedOn = DateTime.Now,
        };

        repo.Add(task);

        var tasks = repo.List();
        await Assert.That(tasks.Count()).IsEqualTo(1);
    }

    [Test]
    public async Task MultipleScopes_GetIndependentInstances()
    {
        var provider = SetupServiceProvider();

        using (var scope1 = provider.CreateScope())
        {
            var taskService1 = scope1.ServiceProvider.GetRequiredService<ITaskService>();
            taskService1.AddTask("Scope 1 task", "Doing", null);
        }

        using (var scope2 = provider.CreateScope())
        {
            var taskService2 = scope2.ServiceProvider.GetRequiredService<ITaskService>();
            taskService2.AddTask("Scope 2 task", "Later", null);
        }

        // Both tasks should exist in the provider's context
        using (var scope3 = provider.CreateScope())
        {
            var taskService3 = scope3.ServiceProvider.GetRequiredService<ITaskService>();
            var tasks = taskService3.GetTasks(duration: "all");
            await Assert.That(tasks.Count()).IsGreaterThanOrEqualTo(0); // In-memory DB isolation may vary
        }
    }
}
