using Dewit.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dewit.Data.Data
{
    public class DewitDbContext : DbContext
    {
        public DewitDbContext(DbContextOptions<DewitDbContext> options)
            : base(options) { }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<ConfigItem> ConfigItems { get; set; }
        public DbSet<MoodEntry> MoodEntries { get; set; }
        public DbSet<MoodDescriptorItem> MoodDescriptors { get; set; }
    }

    public class DewitDbContextFactory : IDesignTimeDbContextFactory<DewitDbContext>
    {
        public DewitDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DewitDbContext>();
            optionsBuilder.UseSqlite(DbConnectionString.Get());
            return new DewitDbContext(optionsBuilder.Options);
        }
    }
}