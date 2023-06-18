using Dewit.Core.Entities;
using Dewit.Core.Enums;

namespace Dewit.Core.Interfaces
{
	interface IJournalService
	{
		IEnumerable<JournalItem> GetJournalLogs();
		void AddJournalEntry(DateTime calendarDate, Moods mood, string? note = null);
		void DeleteJournalEntry(int id);
		void UpdateJournalentry(int id, DateTime updatedOn, Moods mood, string note);
	}
}