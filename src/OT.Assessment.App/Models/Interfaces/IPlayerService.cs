using OT.Assessment.Tester.Infrastructure;

namespace OT.Assessment.App.Models.Interfaces
{
    public interface IPlayerService
    {
        Task<(bool IsSuccess, string ErrorMessage, CasinoWager Wager)> AddWagerAsync(CasinoWager wager);
        Task<(bool IsSuccess, string ErrorMessage, WagerResponseDto WagerResponse)> GetWagersByPlayerAsync(Guid playerId, int page, int pageSize);
        Task<(bool IsSuccess, string ErrorMessage, List<object> TopSpenders)> GetTopSpendersAsync(int count);
    }
}
