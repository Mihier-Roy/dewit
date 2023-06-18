using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;

namespace Dewit.Core.Services
{
	public class JournalService : IJournalService
	{
		private readonly IRepository<JournalItem> _journalRepository;

		public JournalService(IRepository<JournalItem> journalRepository)
		{
			_journalRepository = journalRepository;
		}

		public void AddJournalEntry(DateTime calendarDate, Moods mood, string note)
		{
			_journalRepository.Add(new JournalItem(calendarDate, mood, note ?? ""));
		}

		public void DeleteJournalEntry(int id)
		{
			var item = _journalRepository.GetById(id);
			_journalRepository.Remove(item);
		}

		public IEnumerable<JournalItem> GetJournalLogs()
		{
			return _journalRepository.List();
		}

		public void UpdateJournalentry(int id, DateTime updatedOn, Moods mood, string note)
		{
			var item = _journalRepository.GetById(id);
			item.Mood = mood;
			item.Note = note;
			item.UpdatedOn = updatedOn;
			_journalRepository.Update(item);
		}
	}
}