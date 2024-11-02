using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OT.Assessment.Tester.Infrastructure
{
    public class CasinoWager
    {
        [Key]
        [Column("wagerId")]
        public Guid WagerId { get; set; }

        [ForeignKey("Game")]
        public Guid? GameId { get; set; }

        public virtual Game? Game { get; set; }

        [ForeignKey("Provider")]
        public Guid? ProviderId { get; set; }

        public virtual Provider? Provider { get; set; }

        [ForeignKey("Player")] // Updated to reference Player instead of accountId directly
        public Guid? AccountId { get; set; }

        public virtual Player? Player { get; set; } // Added virtual navigation property for Player

        [Column("externalReferenceId")]
        public Guid? ExternalReferenceId { get; set; }

        [ForeignKey("TransactionType")]
        public Guid? TransactionTypeId { get; set; }

        public virtual TransactionType? TransactionType { get; set; }

        [Column("transactionId")]
        public Guid? TransactionId { get; set; }

        [Column("brandId")]
        public Guid? BrandId { get; set; }

        [Column("amount")]
        public double? Amount { get; set; }

        [Column("username")]
        public string? Username { get; set; }

        [Column("createdDateTime")]
        public DateTime? CreatedDateTime { get; set; }

        [Column("numberOfBets")]
        public int? NumberOfBets { get; set; }

        [Column("countryCode")]
        public string? CountryCode { get; set; }

        [Column("sessionData")]
        public string? SessionData { get; set; }

        [Column("duration")]
        public long? Duration { get; set; }
    }
}
