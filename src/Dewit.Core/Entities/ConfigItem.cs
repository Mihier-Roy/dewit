using System.ComponentModel.DataAnnotations;

namespace Dewit.Core.Entities
{
    public class ConfigItem : EntityBase
    {
        [Required]
        [MaxLength(128)]
        public string Key { get; set; } = string.Empty;

        [Required]
        [MaxLength(1024)]
        public string Value { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}