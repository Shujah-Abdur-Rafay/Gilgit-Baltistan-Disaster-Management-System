using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GBDMS.Data
{
    [Table("Incident")]
    public class Incident
    {
        [PrimaryKey, AutoIncrement]
        [Column("ID")]
        public int Id { get; set; }

        [Column("Type")]
        [Required]
        public string Type { get; set; } = "";

        [Column("Location")]
        [Required]
        public string Location { get; set; } = "";

        [Column("Status")]
        [Required]
        public string Status { get; set; } = "";

        [Column("SeverityLevel")]
        [Required]
        public string SeverityLevel { get; set; } = "";

        [Column("StartTime")]
        [Required]
        public DateTime StartTime { get; set; }

        [Column("Summary")]
        public string Summary { get; set; } = "";

        [Column("CreatedById")]
        public int CreatedById { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("ReportedDate")]
        public DateTime ReportedDate { get; set; } = DateTime.Now;
    }
}