using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dewit.Core.Services
{
    public class MoodService : IMoodService
    {
        private readonly IRepository<MoodEntry> _moodRepo;
        private readonly IRepository<ConfigItem> _configRepo;
        private readonly ILogger<MoodService> _logger;

        public MoodService(
            IRepository<MoodEntry> moodRepo,
            IRepository<ConfigItem> configRepo,
            ILogger<MoodService> logger)
        {
            _moodRepo = moodRepo;
            _configRepo = configRepo;
            _logger = logger;
        }

        public MoodEntry? GetEntryForDate(DateTime date)
        {
            var target = date.Date;
            return _moodRepo.List().FirstOrDefault(e => e.Date == target);
        }

        public IEnumerable<MoodEntry> GetEntriesInRange(DateTime from, DateTime to)
        {
            var start = from.Date;
            var end = to.Date;
            return _moodRepo.List()
                .Where(e => e.Date >= start && e.Date <= end)
                .OrderBy(e => e.Date);
        }

        public void AddEntry(string mood, string descriptors, DateTime date)
        {
            var target = date.Date;

            if (GetEntryForDate(target) != null)
            {
                _logger.LogError("Mood entry already exists for {Date}", target);
                throw new InvalidOperationException($"A mood entry already exists for {target:yyyy-MM-dd}. Use 'mood update' to change it.");
            }

            var entry = new MoodEntry
            {
                Mood = mood,
                Descriptors = descriptors,
                Date = target
            };

            try
            {
                _moodRepo.Add(entry);
                _logger.LogInformation("Added mood entry for {Date}: {Mood}", target, mood);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add mood entry");
                throw new ApplicationException("Failed to add mood entry", ex);
            }
        }

        public void UpdateEntry(DateTime date, string? mood, string? descriptors)
        {
            var target = date.Date;
            var entry = GetEntryForDate(target);

            if (entry == null)
            {
                _logger.LogError("No mood entry found for {Date}", target);
                throw new ApplicationException($"No mood entry found for {target:yyyy-MM-dd}. Use 'mood add' to create one.");
            }

            if (mood != null) entry.Mood = mood;
            if (descriptors != null) entry.Descriptors = descriptors;

            try
            {
                _moodRepo.Update(entry);
                _logger.LogInformation("Updated mood entry for {Date}", target);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update mood entry for {Date}", target);
                throw new ApplicationException($"Failed to update mood entry for {target:yyyy-MM-dd}", ex);
            }
        }

        public IEnumerable<string> GetDescriptors(string mood)
        {
            var key = $"mood.descriptors.{mood.ToLowerInvariant()}";
            var configEntry = _configRepo.List()
                .FirstOrDefault(c => c.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (configEntry == null) return [];

            return configEntry.Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }
}
