using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OT.Assessment.App.Infrastructure
{
    public class Player
    {
        [Key]
        [Column("accountId")]
        public Guid AccountId { get; set; }

        [Required] // Added to ensure username is mandatory
        [Column("username")]
        [MaxLength(50)] // Added to set a reasonable length limit
        public string Username { get; set; }

        [Required] // Added to ensure email is mandatory
        [EmailAddress] // Added to validate email format
        [Column("email")]
        [MaxLength(100)] // Added to set a reasonable length limit
        public string Email { get; set; }

        [Column("createdDate")]
        public DateTime CreatedDate { get; set; } // Nullable removed to enforce creation date
    }
}
