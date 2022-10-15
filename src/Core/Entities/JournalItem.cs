using System;
using System.ComponentModel.DataAnnotations;

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
	}
}