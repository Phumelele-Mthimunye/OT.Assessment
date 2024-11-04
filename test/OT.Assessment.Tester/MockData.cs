using OT.Assessment.App.Infrastructure;
using OT.Assessment.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OT.Assessment.Tester
{
    public class MockData
    {
        public CasinoWager CasinoWagerMockData()
        {
            return new CasinoWager
            {
                WagerId = Guid.NewGuid(),
                GameId = Guid.NewGuid(),
                ProviderId = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                ExternalReferenceId = Guid.NewGuid(),
                TransactionTypeId = Guid.NewGuid(),
                TransactionId = Guid.NewGuid(),
                BrandId = Guid.NewGuid(),
                Amount = 100,
                Username = "TestUser",
                CreatedDateTime = DateTime.UtcNow,
                NumberOfBets = 3,
                CountryCode = "US",
                SessionData = "session-data-string",
                Duration = 120
            };
        }

        public CasinoWager CasinoWagerMockDataAmountFailure()
        {
            return new CasinoWager
            {
                WagerId = Guid.NewGuid(),
                GameId = Guid.NewGuid(),
                ProviderId = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                ExternalReferenceId = Guid.NewGuid(),
                TransactionTypeId = Guid.NewGuid(),
                TransactionId = Guid.NewGuid(),
                BrandId = Guid.NewGuid(),
                Amount = 0,
                Username = "TestUser",
                CreatedDateTime = DateTime.UtcNow,
                NumberOfBets = 3,
                CountryCode = "US",
                SessionData = "session-data-string",
                Duration = 120
            };
        }

        public List<WagerDto> CreateMockWagerDtos(int count)
        {
            var wagerDtos = new List<WagerDto>();

            for (int i = 0; i < count; i++)
            {
                wagerDtos.Add(new WagerDto
                {
                    WagerId = Guid.NewGuid(), // Generate a new GUID for each wager
                    Game = $"Game {i + 1}", // Example game name
                    Provider = $"Provider {i + 1}", // Example provider name
                    Amount = 100.50 + i * 10, // Example wager amount, incrementing by 10
                    CreatedDate = DateTime.UtcNow.AddDays(-i) // Example created date (current UTC time minus i days)
                });
            }

            return wagerDtos;
        }
    }
}
