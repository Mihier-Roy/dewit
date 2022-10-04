using System;
using System.ComponentModel.DataAnnotations;

namespace Dewit.CLI.Models
{
	public enum Moods
	{
		Terrible,
		Bad,
		Meh,
		Okay,
		Great
	}

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