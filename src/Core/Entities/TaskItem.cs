using System.ComponentModel.DataAnnotations;

namespace Dewit.Core.Entities
{
	public class TaskItem : EntityBase
	{
		[Required]
		public string TaskDescription { get; set; }
		[Required]
		public string Status { get; set; }
		[MaxLength(2048)]
		public string Tags { get; set; }
		public DateTime AddedOn { get; set; }
		public DateTime CompletedOn { get; set; }

		public TaskItem(string taskDescription, string status, string tags, DateTime addedOn)
		{
			TaskDescription = taskDescription;
			Status = status;
			Tags = tags;
			AddedOn = addedOn;
		}
	}
}