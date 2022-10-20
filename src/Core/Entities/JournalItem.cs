using Dewit.Core.Enums;

namespace Dewit.Core.Entities
{
	public class JournalItem : EntityBase
	{
		public DateTime CalendarDate { get; set; }
		public Moods Mood { get; set; }
		public string JournalNote { get; set; }
		public DateTime UpdatedOn { get; set; }

		public JournalItem(DateTime calendarDate, Moods mood, string note)
		{
			CalendarDate = calendarDate;
			Mood = mood;
			JournalNote = note;
		}
	}
}