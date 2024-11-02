using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OT.Assessment.Tester.Infrastructure
{
    public class Provider
    {
        [Key]
        [Column("providerId")]
        public Guid ProviderId { get; set; }

        [Required] // Added to ensure name is mandatory
        [Column("name")]
        [MaxLength(100)] // Added to set a reasonable length limit
        public string Name { get; set; }
    }
}
