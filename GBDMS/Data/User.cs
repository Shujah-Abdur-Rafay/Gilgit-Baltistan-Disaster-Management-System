using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBDMS.Data
{
    [Table("User")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        [Column("ID")]
        public int Id { get; set; }

        // Common properties for all users
        [Column("Username")]
        [Required]
        public string Username { get; set; }

        [Column("Email Address")]
        [Required]
        public string Email { get; set; }

        [Column("Password")]
        [Required]
        public string Password { get; set; }

        [Column("Role")]
        public string Role { get; set; } = "User"; // Default role is "User", can be "Admin"

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;
    }
}


