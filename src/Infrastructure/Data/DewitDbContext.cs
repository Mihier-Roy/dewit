using Dewit.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Dewit.Infrastructure.Data
{
	public class DewitDbContext : DbContext
	{
#pragma warning disable CS8618 // Required by Entity Framework
		public DewitDbContext(DbContextOptions<DewitDbContext> opt) : base(opt) { }

		public DbSet<TaskItem> Tasks { get; set; }
		public DbSet<JournalItem> JournalLogs { get; set; }
		public DbSet<ConfigItem> ConfigItems { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}

	public class DewitDbContextFactory : IDesignTimeDbContextFactory<DewitDbContext>
	{
		// Required because dotnet-ef tools does not build migrations without this function.
		public DewitDbContext CreateDbContext(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Path.GetFullPath("../Dewit.CLI"))
				.AddJsonFile("appsettings.json");
			var _config = builder.Build();

			// Setup database
			var optionsBuilder = new DbContextOptionsBuilder<DewitDbContext>();
			optionsBuilder.UseSqlite(_config.GetConnectionString("Sqlite"));
			return new DewitDbContext(optionsBuilder.Options);
		}
	}
}