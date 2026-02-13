using Dewit.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Dewit.Data.Data
{
    public class DewitDbContext : DbContext
    {
        public DewitDbContext(DbContextOptions<DewitDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<ConfigItem> ConfigItems { get; set; }
    }

    public class DewitDbContextFactory : IDesignTimeDbContextFactory<DewitDbContext>
    {
        public DewitDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false);
            var config = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<DewitDbContext>();
            optionsBuilder.UseSqlite(config.GetConnectionString("Sqlite"));
            return new DewitDbContext(optionsBuilder.Options);
        }
    }
}
