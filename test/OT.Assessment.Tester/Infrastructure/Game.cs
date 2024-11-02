using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OT.Assessment.Tester.Infrastructure
{
    public class Game
    {
        [Key]
        [Column("gameId")]
        public Guid GameId { get; set; }

        [Required] // Added to ensure name is mandatory
        [Column("name")]
        [MaxLength(100)] // Added to set a reasonable length limit
        public string Name { get; set; }

        [Column("theme")]
        [MaxLength(50)] // Added to set a reasonable length limit
        public string? Theme { get; set; }

        [ForeignKey("Provider")] // Updated to remove redundant providerId
        public Guid? ProviderId { get; set; }

        public virtual Provider? Provider { get; set; } // Simplified FK reference
    }
}
