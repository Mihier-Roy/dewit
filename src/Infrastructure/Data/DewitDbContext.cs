using Dewit.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Dewit.Infrastructure.Data
{
	public class DewitDbContext : DbContext
	{
#pragma warning disable CS8618 // Required by Entity Framework
		public DewitDbContext(DbContextOptions<DewitDbContext> opt) : base(opt) { }

		public DbSet<TaskItem> Tasks { get; set; }
		public DbSet<JournalItem> JournalLogs { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}

	// public class TaskContextFactory : IDesignTimeDbContextFactory<TaskContext>
	// {
	// 	// Required because dotnet-ef tools does not build migrations without this function.
	// 	public TaskContext CreateDbContext(string[] args)
	// 	{
	// 		// Load config file manually.
	// 		var builder = new ConfigurationBuilder()
	// 							.SetBasePath(Directory.GetCurrentDirectory())
	// 							.AddJsonFile("config.json");
	// 		var _config = builder.Build();

	// 		// Setup database
	// 		var optionsBuilder = new DbContextOptionsBuilder<TaskContext>();
	// 		optionsBuilder.UseSqlite(_config.GetConnectionString("Sqlite"));
	// 		return new TaskContext(optionsBuilder.Options);
	// 	}
	// }
}