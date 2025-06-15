using SQLite;
using System;

namespace GBDMS.Models
{
    [Table("DisasterAlerts")]
    public class DisasterAlert
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Type { get; set; } = string.Empty;

        [NotNull]
        public string Severity { get; set; } = string.Empty;

        [NotNull]
        public string Area { get; set; } = string.Empty;

        [NotNull]
        public string Message { get; set; } = string.Empty;

        public DateTime Time { get; set; } = DateTime.Now;

        public double RiskScore { get; set; } = 0;

        public string RiskParameters { get; set; } = string.Empty; // JSON string of parameters

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; } = "System";
    }
}
