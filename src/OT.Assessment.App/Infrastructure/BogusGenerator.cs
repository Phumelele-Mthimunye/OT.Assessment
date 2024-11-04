using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OT.Assessment.App.Infrastructure
{
    public class BogusGenerator
    {
        private readonly Faker<CasinoWager> _casinoWagerFaker;
        private readonly List<Player> _players;
        private readonly List<Provider> _providers;
        private readonly List<Game> _games;
        private readonly List<TransactionType> _transactionTypes;

        public BogusGenerator()
        {
            Randomizer.Seed = new Random(1010);

            // Themes for games
            var themes = new[] { "ancient", "adventure", "wildlife", "jungle", "retro", "family", "crash" };

            // Generate Players
            _players = new Faker<Player>()
                .StrictMode(true)
                .RuleFor(o => o.AccountId, f => f.Random.Guid())
                .RuleFor(o => o.Username, f => f.Person.UserName)
                .RuleFor(o => o.Email, f => f.Internet.Email())
                .RuleFor(o => o.CreatedDate, f => f.Date.Past(2))
                .Generate(1000);

            // Generate Providers and their Games
            _providers = new Faker<Provider>()
                .StrictMode(true)
                .RuleFor(o => o.ProviderId, f => f.Random.Guid())
                .RuleFor(o => o.Name, f => f.Company.CompanyName())
                .Generate(100);

            // Generate Games associated with Providers
            _games = _providers.SelectMany(provider =>
                new Faker<Game>()
                    .StrictMode(true)
                    .RuleFor(g => g.GameId, f => f.Random.Guid())
                    .RuleFor(g => g.Name, f => f.Commerce.ProductName())
                    .RuleFor(g => g.Theme, f => f.PickRandom(themes))
                    .RuleFor(g => g.ProviderId, _ => provider.ProviderId)
                    .Generate(10))
                .ToList();

            // Generate TransactionTypes
            _transactionTypes = new Faker<TransactionType>()
                .StrictMode(true)
                .RuleFor(t => t.TransactionTypeId, f => f.Random.Guid())
                .RuleFor(t => t.Name, f => f.Commerce.Department())
                .Generate(5);

            // Generate CasinoWagers
            _casinoWagerFaker = new Faker<CasinoWager>()
                .StrictMode(true)
                .RuleFor(o => o.WagerId, f => f.Random.Guid())
                .RuleFor(o => o.SessionData, f => f.Random.Words(20))
                .RuleFor(o => o.Provider, f => f.PickRandom(_providers))
                .RuleFor(o => o.Game, (f, u) => f.PickRandom(_games.Where(g => g.ProviderId == u.Provider.ProviderId)))
                .RuleFor(o => o.TransactionType, f => f.PickRandom(_transactionTypes))
                .RuleFor(o => o.TransactionId, f => f.Random.Guid())
                .RuleFor(o => o.BrandId, f => f.Random.Guid())
                .RuleFor(o => o.Username, f => f.PickRandom(_players).Username)
                .RuleFor(o => o.AccountId, (f, u) => _players.First(p => p.Username == u.Username).AccountId)
                .RuleFor(o => o.ExternalReferenceId, f => f.Random.Guid())
                .RuleFor(o => o.CreatedDateTime, f => f.Date.Past())
                .RuleFor(o => o.NumberOfBets, f => f.Random.Int(1, 10))
                .RuleFor(o => o.CountryCode, f => f.Address.CountryCode())
                .RuleFor(o => o.Duration, f => f.Random.Long(10000, 3600000))
                .RuleFor(o => o.Amount, f => f.Random.Double(10, 50000));
        }

        public List<CasinoWager> Generate(int count = 10000)
        {
            return _casinoWagerFaker.Generate(count);
        }

        public List<Player> GetPlayers()
        {
            return _players;
        }

        public List<Provider> GetProviders()
        {
            return _providers;
        }

        public List<Game> GetGames()
        {
            return _games;
        }

        public List<TransactionType> GetTransactionTypes()
        {
            return _transactionTypes;
        }
    }
}
