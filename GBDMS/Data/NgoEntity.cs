using SQLite;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GBDMS.Data
{
    [Table("NGO")]
    public class NgoEntity
    {
        [PrimaryKey, AutoIncrement]
        [Column("ID")]
        public int Id { get; set; }

        [Column("Name")]
        [Required]
        public string Name { get; set; } = "";

        [Column("Type")]
        [Required]
        public string Type { get; set; } = "";

        [Column("District")]
        [Required]
        public string District { get; set; } = "";

        [Column("Location")]
        public string Location { get; set; } = "";

        [Column("PrimaryFocusArea")]
        [Required]
        public string PrimaryFocusArea { get; set; } = "";

        [Column("SecondaryFocusAreas")]
        public string SecondaryFocusAreasString { get; set; } = "";

        [Ignore]
        public List<string> SecondaryFocusAreas 
        { 
            get => string.IsNullOrEmpty(SecondaryFocusAreasString) 
                ? new List<string>() 
                : SecondaryFocusAreasString.Split(',').ToList();
            set => SecondaryFocusAreasString = string.Join(",", value);
        }

        [Column("ContactPhone")]
        public string ContactPhone { get; set; } = "";

        [Column("Email")]
        public string Email { get; set; } = "";

        [Column("Website")]
        public string Website { get; set; } = "";

        [Column("Description")]
        public string Description { get; set; } = "";

        [Column("Latitude")]
        public double Latitude { get; set; }

        [Column("Longitude")]
        public double Longitude { get; set; }
    }
} 