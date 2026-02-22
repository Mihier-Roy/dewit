using Dewit.Core.Entities;

namespace Dewit.Core.Interfaces
{
    public interface IJournalService
    {
        JournalEntry? GetEntryForDate(DateTime date);
        IEnumerable<JournalEntry> GetEntriesInRange(DateTime from, DateTime to);
        JournalEntry CreateOrGetEntry(DateTime date, string moodName, string descriptors);
        void TouchUpdatedAt(DateTime date);
        string GetFilePath(DateTime date);
    }
}