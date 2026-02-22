using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Dewit.Core.Utils;

namespace Dewit.Core.Services
{
    public class MoodService : IMoodService
    {
        private readonly IRepository<MoodEntry> _moodRepo;
        private readonly IRepository<MoodDescriptorItem> _descriptorRepo;

        public MoodService(
            IRepository<MoodEntry> moodRepo,
            IRepository<MoodDescriptorItem> descriptorRepo
        )
        {
            _moodRepo = moodRepo;
            _descriptorRepo = descriptorRepo;
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
            return _moodRepo
                .List()
                .Where(e => e.Date >= start && e.Date <= end)
                .OrderBy(e => e.Date);
        }

        public void AddEntry(string mood, string descriptors, DateTime date)
        {
            var target = date.Date;

            if (GetEntryForDate(target) != null)
                throw new InvalidOperationException(
                    $"A mood entry already exists for {target:yyyy-MM-dd}. Use 'mood update' to change it."
                );

            var entry = new MoodEntry
            {
                Mood = mood,
                Descriptors = descriptors,
                Date = target,
            };

            try
            {
                _moodRepo.Add(entry);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to add mood entry", ex);
            }
        }

        public void UpdateEntry(DateTime date, string? mood, string? descriptors)
        {
            var target = date.Date;
            var entry = GetEntryForDate(target);

            if (entry == null)
                throw new ApplicationException(
                    $"No mood entry found for {target:yyyy-MM-dd}. Use 'mood add' to create one."
                );

            if (mood != null)
                entry.Mood = mood;
            if (descriptors != null)
                entry.Descriptors = descriptors;

            try
            {
                _moodRepo.Update(entry);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Failed to update mood entry for {target:yyyy-MM-dd}",
                    ex
                );
            }
        }

        public IEnumerable<string> GetDescriptors(string mood)
        {
            var row = _descriptorRepo
                .List()
                .FirstOrDefault(d => d.Mood.Equals(mood, StringComparison.OrdinalIgnoreCase));

            if (row == null)
                return [];

            return row.Descriptors.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
        }

        public IEnumerable<MoodDescriptorItem> GetAllDescriptors()
        {
            return _descriptorRepo.List().OrderBy(d => d.Mood);
        }

        public void SetDescriptors(string mood, string descriptors)
        {
            var existing = _descriptorRepo
                .List()
                .FirstOrDefault(d => d.Mood.Equals(mood, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.Descriptors = descriptors;
                _descriptorRepo.Update(existing);
            }
            else
            {
                _descriptorRepo.Add(
                    new MoodDescriptorItem { Mood = mood, Descriptors = descriptors }
                );
            }
        }

        public void ResetDescriptors(string mood)
        {
            if (!Enum.TryParse<Mood>(mood, ignoreCase: true, out var moodEnum))
                throw new ArgumentException($"Unknown mood: {mood}");

            if (!MoodDescriptorDefaults.Defaults.TryGetValue(moodEnum, out var defaults))
                throw new ArgumentException($"No defaults found for mood: {mood}");

            SetDescriptors(moodEnum.ToString(), defaults);
        }
    }
}