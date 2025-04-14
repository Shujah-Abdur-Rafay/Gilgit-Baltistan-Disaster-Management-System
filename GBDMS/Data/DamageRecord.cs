using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GBDMS.Data
{
    [Table("DamageRecord")]
    public class DamageRecord
    {
        [PrimaryKey, AutoIncrement]
        [Column("ID")]
        public int Id { get; set; }

        [Column("Title")]
        [Required]
        public string Title { get; set; } = "";

        [Column("DisasterType")]
        [Required]
        public string DisasterType { get; set; } = "";

        [Column("District")]
        [Required]
        public string District { get; set; } = "";

        [Column("Location")]
        public string Location { get; set; } = "";

        [Column("Date")]
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        [Column("Category")]
        [Required]
        public string Category { get; set; } = "";

        [Column("FinancialLoss")]
        public double FinancialLoss { get; set; } = 0;

        [Column("Severity")]
        public string Severity { get; set; } = "Medium";

        [Column("Description")]
        public string Description { get; set; } = "";

        [Column("IsVerified")]
        public bool IsVerified { get; set; } = false;
    }
} 