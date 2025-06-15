using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GBDMS.Models
{
    [Table("SurvivalGuidelines")]
    public class SurvivalGuideline
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        [Required]
        public string Title { get; set; } = string.Empty;

        [NotNull]
        [Required]
        public string VideoUrl { get; set; } = string.Empty;

        [NotNull]
        [Required]
        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty; // e.g., "Earthquake", "Flood", "Landslide"

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; } = "Admin";
    }
}
