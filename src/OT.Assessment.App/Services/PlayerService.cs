using OT.Assessment.App.Models;
using OT.Assessment.App.Models.Interfaces;
using OT.Assessment.App.Infrastructure;

namespace OT.Assessment.App.Services
{
    // The PlayerService class implements IPlayerService to handle operations related to player wagers
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerAdapter _playerAdapter; // Dependency on the player adapter interface

        // Constructor for PlayerService that takes an IPlayerAdapter as a parameter
        public PlayerService(IPlayerAdapter playerAdapter)
        {
            _playerAdapter = playerAdapter;
        }

        public async Task<(bool IsSuccess, string ErrorMessage, CasinoWager Wager)> AddWagerAsync(CasinoWager wager)
        {
            // Validate the wager input
            if (wager == null || wager.AccountId == Guid.Empty)
            {
                return (false, "Invalid wager data.", null); // Return error if wager is null or has an invalid AccountId
            }

            if (wager.Amount <= 0)
            {
                return (false, "Invalid wager. Amount must be greater than 0.", null); // Return error if amount is not valid
            }

            try
            {
                _playerAdapter.PublishWager(wager); // Publish the wager to RabbitMQ
                await _playerAdapter.AddWagerAsync(wager); // Add the wager to the database asynchronously
                return (true, null, wager); // Return success if both operations are successful
            }
            catch (Exception ex)
            {
                // Catch any exceptions and return an error message
                return (false, $"An error occurred while processing the wager: {ex.Message}", null);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage, WagerResponseDto WagerResponse)> GetWagersByPlayerAsync(Guid playerId, int page = 1, int pageSize = 10)
        {
            // Validate the player ID input
            if (playerId == Guid.Empty)
            {
                return (false, "Invalid player ID.", null); // Return error if player ID is invalid
            }

            try
            {
                // Call the adapter to get wagers, including total count and pages for pagination
                var (wagers, total, totalPages) = await _playerAdapter.GetWagersByPlayerAsync(playerId, page, pageSize);

                // Check if any wagers were found
                if (wagers.Count == 0)
                {
                    return (false, "No wagers found for this player.", null); // Return error if no wagers exist for the player
                }

                // Construct the response object to return
                var response = new WagerResponseDto
                {
                    Data = wagers,
                    Page = page, 
                    PageSize = pageSize, 
                    Total = total,
                    TotalPages = totalPages
                };

                return (true, null, response); // Return success with the response object
            }
            catch (Exception ex)
            {
                // Catch any exceptions and return an error message
                return (false, $"An error occurred while retrieving wagers: {ex.Message}", null);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage, List<object> TopSpenders)> GetTopSpendersAsync(int count)
        {
            // Validate the count input
            if (count <= 0)
            {
                return (false, "Count must be greater than zero.", null); // Return error if count is not valid
            }

            try
            {
                // Call the adapter to get the top spenders
                var topSpenders = await _playerAdapter.GetTopSpendersAsync(count);
                return (true, null, topSpenders); // Return success with the list of top spenders
            }
            catch (Exception ex)
            {
                // Catch any exceptions and return an error message
                return (false, $"An error occurred while retrieving top spenders: {ex.Message}", null);
            }
        }
    }
}
