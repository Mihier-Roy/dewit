using System.Collections.Generic;
using Dewit.CLI.Models;

namespace Dewit.CLI.Data
{
	public interface IJournalRepository
	{
		IEnumerable<JournalItem> GetJournalItems();
		JournalItem GetJournalById(int id);
		void AddJournalItem(JournalItem journalItem);
		void UpdateJournalItem(JournalItem journalItem);
		void RemoveJournalItem(JournalItem journalItem);
		bool SaveChanges();
	}
}