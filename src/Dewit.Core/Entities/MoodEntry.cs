using System.ComponentModel.DataAnnotations;

namespace Dewit.Core.Entities
{
    public class MoodEntry : EntityBase
    {
        [Required]
        [MaxLength(32)]
        public string Mood { get; set; } = string.Empty;       // Stores enum name e.g. "VeryHappy"

        [MaxLength(1024)]
        public string Descriptors { get; set; } = string.Empty; // Comma-separated e.g. "inspired,valued"

        public DateTime Date { get; set; }                      // Date portion only (time = midnight)
    }
}
