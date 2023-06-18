namespace Dewit.Core.Entities
{
	public class TaskItem : EntityBase
	{
		public string TaskDescription { get; set; }
		public string Status { get; set; }
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