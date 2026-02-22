using System.ComponentModel.DataAnnotations;

namespace Dewit.Core.Entities
{
    public class JournalEntry : EntityBase
    {
        public DateTime Date { get; set; }

        [Required, MaxLength(512)]
        public string FilePath { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}