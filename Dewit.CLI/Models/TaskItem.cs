using System;

namespace Dewit.CLI.Models
{
	public class TaskItem
	{
		public int Id { get; set; }
		public string TaskDescription { get; set; }
		public DateTime AddedOn { get; set; }
		public DateTime CompletedOn { get; set; }
	}
}