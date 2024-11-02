using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OT.Assessment.Tester.Infrastructure
{
    public class TransactionType
    {
        [Key]
        [Column("transactionTypeId")]
        public Guid TransactionTypeId { get; set; }

        [Required] // Added to ensure name is mandatory
        [Column("name")]
        [MaxLength(50)] // Added to set a reasonable length limit
        public string Name { get; set; }
    }
}
