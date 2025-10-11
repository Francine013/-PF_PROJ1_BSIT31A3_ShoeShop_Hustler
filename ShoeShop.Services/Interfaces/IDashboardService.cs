using ShoeShop.Services.DTOs;
using System.Threading.Tasks;

namespace ShoeShop.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
    }
}
