using Dewit.Core.Enums;
using Dewit.Core.Interfaces;

namespace Dewit.Core.Services
{
	public class JournalService : IJournalService
	{
		public void AddJournalEntry(DateTime calendarDate, Moods mood, string? note = null)
		{
			throw new NotImplementedException();
		}

		public void DeleteJournalEntry(int id)
		{
			throw new NotImplementedException();
		}

		public void GetJournalLogs()
		{
			throw new NotImplementedException();
		}

		public void UpdateJournalentry(int id, DateTime updatedOn, Moods mood, string note)
		{
			throw new NotImplementedException();
		}
	}
}