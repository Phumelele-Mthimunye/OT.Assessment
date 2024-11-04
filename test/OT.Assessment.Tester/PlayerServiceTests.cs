using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using OT.Assessment.App.Infrastructure;
using OT.Assessment.App.Models;
using OT.Assessment.App.Models.Interfaces;
using OT.Assessment.App.Services;
using Xunit;

namespace OT.Assessment.Tester.Services
{
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerAdapter> _mockPlayerAdapter;
        private readonly PlayerService _playerService;
        private readonly MockData _mockData;

        public PlayerServiceTests()
        {
            _mockPlayerAdapter = new Mock<IPlayerAdapter>();
            _playerService = new PlayerService(_mockPlayerAdapter.Object);
            _mockData = new MockData();
        }

        [Fact]
        public async Task AddWagerAsync_InvalidWager_ReturnsFailure()
        {
            // Arrange
            CasinoWager invalidWager = null;

            // Act
            var result = await _playerService.AddWagerAsync(invalidWager);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid wager data.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddWagerAsync_InvalidAmount_ReturnsFailure()
        {
            // Arrange
            var wager = _mockData.CasinoWagerMockDataAmountFailure();

            // Act
            var result = await _playerService.AddWagerAsync(wager);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid wager. Amount must be greater than 0.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddWagerAsync_ValidWager_ReturnsSuccess()
        {
            // Arrange
            var wager = _mockData.CasinoWagerMockData();
            _mockPlayerAdapter.Setup(x => x.AddWagerAsync(It.IsAny<CasinoWager>()))
                              .Returns(Task.FromResult(wager));

            // Act
            var result = await _playerService.AddWagerAsync(wager);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrorMessage);
            Assert.Equal(wager, result.Wager);
        }

        [Fact]
        public async Task GetWagersByPlayerAsync_InvalidPlayerId_ReturnsFailure()
        {
            // Arrange
            Guid invalidPlayerId = Guid.Empty;

            // Act
            var result = await _playerService.GetWagersByPlayerAsync(invalidPlayerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid player ID.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetWagersByPlayerAsync_NoWagersFound_ReturnsFailure()
        {
            // Arrange
            var playerId = Guid.NewGuid();

            // Setup the mock to return an empty list of wagers
            var emptyWagerList = new List<WagerDto>();
            _mockPlayerAdapter.Setup(x => x.GetWagersByPlayerAsync(playerId, 1, 10))
                .Returns(Task.FromResult((emptyWagerList, 0, 0))); // Returning an empty list, total of 0, and total pages of 0

            // Act
            var result = await _playerService.GetWagersByPlayerAsync(playerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No wagers found for this player.", result.ErrorMessage);
        }



        [Fact]
        public async Task GetWagersByPlayerAsync_ValidPlayerId_ReturnsSuccess()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var wagers = _mockData.CreateMockWagerDtos(3);
            _mockPlayerAdapter.Setup(x => x.GetWagersByPlayerAsync(playerId, 1, 10)).Returns(Task.FromResult((wagers, 1, 10)));

            // Act
            var result = await _playerService.GetWagersByPlayerAsync(playerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.WagerResponse);
            Assert.Equal(1, result.WagerResponse.Total);
        }

        [Fact]
        public async Task GetTopSpendersAsync_InvalidCount_ReturnsFailure()
        {
            // Arrange
            int invalidCount = 0;

            // Act
            var result = await _playerService.GetTopSpendersAsync(invalidCount);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Count must be greater than zero.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetTopSpendersAsync_ValidCount_ReturnsSuccess()
        {
            // Arrange
            int validCount = 5;
            var topSpenders = new List<object> { new { PlayerId = Guid.NewGuid(), Amount = 500 } };
            _mockPlayerAdapter.Setup(x => x.GetTopSpendersAsync(validCount))
                .ReturnsAsync(topSpenders);

            // Act
            var result = await _playerService.GetTopSpendersAsync(validCount);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrorMessage);
            Assert.Equal(topSpenders, result.TopSpenders);
        }
    }
}
