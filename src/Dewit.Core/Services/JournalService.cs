using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Dewit.Core.Utils;

namespace Dewit.Core.Services
{
    public class JournalService : IJournalService
    {
        private readonly IRepository<JournalEntry> _repo;
        private readonly string _baseDir;

        public JournalService(IRepository<JournalEntry> repo, string? baseDir = null)
        {
            _repo = repo;
            _baseDir = baseDir ?? DewitDirectory.GetBaseDir();
        }

        public string GetFilePath(DateTime date) =>
            Path.Combine(_baseDir, date.Year.ToString(), date.ToString("MM-dd") + ".md");

        public JournalEntry? GetEntryForDate(DateTime date)
        {
            var target = date.Date;
            return _repo.List().FirstOrDefault(e => e.Date == target);
        }

        public IEnumerable<JournalEntry> GetEntriesInRange(DateTime from, DateTime to)
        {
            var start = from.Date;
            var end = to.Date;
            return _repo.List().Where(e => e.Date >= start && e.Date <= end).OrderBy(e => e.Date);
        }

        public JournalEntry CreateOrGetEntry(DateTime date, string moodName, string descriptors)
        {
            var target = date.Date;
            var existing = GetEntryForDate(target);
            if (existing != null)
                return existing;

            var filePath = GetFilePath(target);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            var frontmatter = BuildFrontmatter(target, moodName, descriptors);
            File.WriteAllText(filePath, frontmatter);

            var entry = new JournalEntry
            {
                Date = target,
                FilePath = filePath,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            try
            {
                _repo.Add(entry);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create journal entry", ex);
            }

            return entry;
        }

        public void TouchUpdatedAt(DateTime date)
        {
            var target = date.Date;
            var entry = GetEntryForDate(target);
            if (entry == null)
                return;

            entry.UpdatedAt = DateTime.UtcNow;

            try
            {
                _repo.Update(entry);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update journal entry timestamp", ex);
            }
        }

        private static string BuildFrontmatter(DateTime date, string moodName, string descriptors)
        {
            var lines = new List<string> { "---", $"date: {date:yyyy-MM-dd}", $"mood: {moodName}" };

            if (!string.IsNullOrWhiteSpace(descriptors))
                lines.Add($"mood-descriptors: {descriptors}");

            lines.Add("---");
            lines.Add(string.Empty);

            return string.Join(Environment.NewLine, lines);
        }
    }
}