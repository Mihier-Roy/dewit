using Dewit.Core.Entities;

namespace Dewit.Core.Interfaces
{
    public interface IMoodService
    {
        /// <summary>Returns null if no entry exists for that date.</summary>
        MoodEntry? GetEntryForDate(DateTime date);

        IEnumerable<MoodEntry> GetEntriesInRange(DateTime from, DateTime to);

        /// <summary>Throws InvalidOperationException if an entry already exists for that date.</summary>
        void AddEntry(string mood, string descriptors, DateTime date);

        /// <summary>Throws ApplicationException if no entry exists for the given date.</summary>
        void UpdateEntry(DateTime date, string? mood, string? descriptors);

        /// <summary>Returns descriptors for a mood from the MoodDescriptors table, falling back to empty list.</summary>
        IEnumerable<string> GetDescriptors(string mood);

        IEnumerable<MoodDescriptorItem> GetAllDescriptors();

        void SetDescriptors(string mood, string descriptors);

        /// <summary>Resets descriptors for the given mood to the built-in defaults.</summary>
        void ResetDescriptors(string mood);
    }
}
