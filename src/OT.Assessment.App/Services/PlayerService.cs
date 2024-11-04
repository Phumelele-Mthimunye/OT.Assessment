using OT.Assessment.App.Models;
using OT.Assessment.App.Models.Interfaces;
using OT.Assessment.Tester.Infrastructure;

namespace OT.Assessment.App.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerAdapter _playerAdapter;

        public PlayerService(IPlayerAdapter playerAdapter)
        {
            _playerAdapter = playerAdapter;
        }

        public async Task<(bool IsSuccess, string ErrorMessage, CasinoWager Wager)> AddWagerAsync(CasinoWager wager)
        {
            if (wager == null || wager.AccountId == Guid.Empty)
            {
                return (false, "Invalid wager data.", null);
            }

            if (wager.Amount <= 0)
            {
                return (false, "Invalid wager. Amount must be greater than 0.", null);
            }

            try
            {
                _playerAdapter.PublishWager(wager);
                await _playerAdapter.AddWagerAsync(wager);
                return (true, null, wager);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while processing the wager: {ex.Message}", null);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage, WagerResponseDto WagerResponse)> GetWagersByPlayerAsync(Guid playerId, int page = 1, int pageSize = 10)
        {
            if (playerId == Guid.Empty)
            {
                return (false, "Invalid player ID.", null);
            }

            try
            {
                var (wagers, total, totalPages) = await _playerAdapter.GetWagersByPlayerAsync(playerId, page, pageSize);
                if (wagers.Count == 0)
                {
                    return (false, "No wagers found for this player.", null);
                }

                // Construct the response object
                var response = new WagerResponseDto
                {
                    Data = wagers,
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    TotalPages = totalPages
                };

                return (true, null, response);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while retrieving wagers: {ex.Message}", null);
            }
        }


        public async Task<(bool IsSuccess, string ErrorMessage, List<object> TopSpenders)> GetTopSpendersAsync(int count)
        {
            if (count <= 0)
            {
                return (false, "Count must be greater than zero.", null);
            }

            try
            {
                var topSpenders = await _playerAdapter.GetTopSpendersAsync(count);
                return (true, null, topSpenders);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while retrieving top spenders: {ex.Message}", null);
            }
        }
    }
}
