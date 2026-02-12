using System;
using System.ComponentModel.DataAnnotations;

namespace Dewit.CLI.Models
{
    public class TaskItem
    {
        [Key]
        public int? Id { get; set; }
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
