using BookStore.Service.DTOs;
using System.Threading.Tasks;

namespace BookStore.Service.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<DashboardStatsDto> GetDashboardStatsAsync(DateTime? startDate, DateTime? endDate);
    }
}