using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace GBDMS.Data
{
    [Table("InventoryItem")]
    public class InventoryItem
    {
        [PrimaryKey, AutoIncrement]
        [Column("ID")]
        public int Id { get; set; }

        [Column("ItemCode")]
        [Required]
        public string ItemCode { get; set; } = "";

        [Column("Name")]
        [Required]
        public string Name { get; set; } = "";

        [Column("Category")]
        [Required]
        public string Category { get; set; } = "";

        [Column("Quantity")]
        public int Quantity { get; set; }

        [Column("Unit")]
        public string Unit { get; set; } = "";

        [Column("MinimumLevel")]
        public int MinimumLevel { get; set; }

        [Column("LastUpdated")]
        public DateTime LastUpdated { get; set; }
    }
} 