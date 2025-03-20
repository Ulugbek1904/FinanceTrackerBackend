using FinanceTracker.Domain.Models.DTOs;

namespace FinanceTracker.Services.Processings
{
    public interface IDashboardProcessingService
    {
        DashboardData GetDashboardData(Guid userId);
    }
}
