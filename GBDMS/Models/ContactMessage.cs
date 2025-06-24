using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GBDMS.Models
{
    [Table("ContactMessages")]
    public class ContactMessage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        [Required]
        public string FullName { get; set; } = string.Empty;

        [NotNull]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [NotNull]
        [Required]
        public string ContactNumber { get; set; } = string.Empty;

        [NotNull]
        [Required]
        public string Remarks { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;

        public string Status { get; set; } = "New"; // New, In Progress, Resolved

        public string AdminResponse { get; set; } = string.Empty;

        public DateTime? RespondedAt { get; set; }
    }
}
