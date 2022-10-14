using System.IO;
using Dewit.CLI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Dewit.CLI.Data
{
	public class DewitDbContext : DbContext
	{
		public DewitDbContext(DbContextOptions<DewitDbContext> opt) : base(opt) { }

		public DbSet<TaskItem> Tasks { get; set; }
		public DbSet<JournalItem> JournalLogs { get; set; }
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