using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Dewit.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Dewit.Core.Services
{
    public class MoodService : IMoodService
    {
        private readonly IRepository<MoodEntry> _moodRepo;
        private readonly IRepository<MoodDescriptorItem> _descriptorRepo;
        private readonly ILogger<MoodService> _logger;

        public MoodService(
            IRepository<MoodEntry> moodRepo,
            IRepository<MoodDescriptorItem> descriptorRepo,
            ILogger<MoodService> logger)
        {
            _moodRepo = moodRepo;
            _descriptorRepo = descriptorRepo;
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
            var row = _descriptorRepo.List()
                .FirstOrDefault(d => d.Mood.Equals(mood, StringComparison.OrdinalIgnoreCase));

            if (row == null) return [];

            return row.Descriptors
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        public IEnumerable<MoodDescriptorItem> GetAllDescriptors()
        {
            return _descriptorRepo.List().OrderBy(d => d.Mood);
        }

        public void SetDescriptors(string mood, string descriptors)
        {
            var existing = _descriptorRepo.List()
                .FirstOrDefault(d => d.Mood.Equals(mood, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.Descriptors = descriptors;
                _descriptorRepo.Update(existing);
            }
            else
            {
                _descriptorRepo.Add(new MoodDescriptorItem { Mood = mood, Descriptors = descriptors });
            }

            _logger.LogInformation("Updated descriptors for mood {Mood}", mood);
        }

        public void ResetDescriptors(string mood)
        {
            if (!Enum.TryParse<Mood>(mood, ignoreCase: true, out var moodEnum))
                throw new ArgumentException($"Unknown mood: {mood}");

            if (!MoodDescriptorDefaults.Defaults.TryGetValue(moodEnum, out var defaults))
                throw new ArgumentException($"No defaults found for mood: {mood}");

            SetDescriptors(moodEnum.ToString(), defaults);
            _logger.LogInformation("Reset descriptors for mood {Mood} to defaults", mood);
        }
    }
}
