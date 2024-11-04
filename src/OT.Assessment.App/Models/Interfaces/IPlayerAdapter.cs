using OT.Assessment.App .Infrastructure;

namespace OT.Assessment.App.Models.Interfaces
{
    public interface IPlayerAdapter : IDisposable
    {
        Task AddWagerAsync(CasinoWager wager);
        Task<(List<WagerDto> Data, int Total, int TotalPages)> GetWagersByPlayerAsync(Guid playerId, int page, int pageSize);
        Task<List<object>> GetTopSpendersAsync(int count);
        void PublishWager(CasinoWager wager);
    }
}
