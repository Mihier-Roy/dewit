using System;
using System.ComponentModel.DataAnnotations;

enum Moods
{
	Terrible,
	Bad,
	Meh,
	Okay,
	Great
}

namespace Dewit.CLI.Models
{
	public class JournalItem
	{
		[Key]
		public int? Id { get; set; }
		[Required]
		public DateTime CalendarDate { get; set; }
		[Required]
		public Moods Mood { get; set; }
		public string JournalNote { get; set; }
		public DateTime UpdatedOn { get; set; }
	}
}