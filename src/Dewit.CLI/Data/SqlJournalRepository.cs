using System;
using System.Collections.Generic;
using Dewit.CLI.Models;

namespace Dewit.CLI.Data
{
	public class SqlJournalRepository : IJournalRepository
	{
		public IEnumerable<JournalItem> GetJournalItems()
		{
			throw new NotImplementedException();
		}
		public JournalItem GetJournalById(int id)
		{
			throw new NotImplementedException();
		}
		public void AddJournalItem(JournalItem journalItem)
		{
			throw new NotImplementedException();
		}
		public void UpdateJournalItem(JournalItem journalItem)
		{
			throw new NotImplementedException();
		}
		public void RemoveJournalItem(JournalItem journalItem)
		{
			throw new NotImplementedException();
		}
		public bool SaveChanges()
		{
			throw new NotImplementedException();
		}
	}
}