using System.ComponentModel.DataAnnotations;
using Dewit.Core.Enums;

namespace Dewit.Core.Entities
{
	public class JournalItem : EntityBase
	{
		[Required]
		public DateTime CalendarDate { get; set; }
		[Required]
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