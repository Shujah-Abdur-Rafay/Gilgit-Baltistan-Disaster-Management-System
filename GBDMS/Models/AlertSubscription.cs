using SQLite;
using System;

namespace GBDMS.Models
{
    [Table("AlertSubscriptions")]
    public class AlertSubscription
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Email { get; set; } = string.Empty;

        [NotNull]
        public string District { get; set; } = string.Empty;

        public DateTime SubscribedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
