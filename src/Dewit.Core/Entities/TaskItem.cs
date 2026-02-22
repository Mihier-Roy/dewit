using System.ComponentModel.DataAnnotations;

namespace Dewit.Core.Entities
{
    public class TaskItem : EntityBase
    {
        [Required]
        public string TaskDescription { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;

        [MaxLength(2048)]
        public string Tags { get; set; } = string.Empty;
        public DateTime AddedOn { get; set; }
        public DateTime CompletedOn { get; set; }
    }
}