using ShoeShop.Services.DTOs;

namespace ShoeShop.Services.Interfaces
{
    public interface IPullOutService
    {
        Task<PullOutRequestDto> CreatePullOutRequestAsync(CreatePullOutDto dto);
        Task ApprovePullOutAsync(int pullOutId, string approvedBy);
        Task<IEnumerable<PullOutRequestDto>> GetPendingPullOutsAsync();
        
    }
}

