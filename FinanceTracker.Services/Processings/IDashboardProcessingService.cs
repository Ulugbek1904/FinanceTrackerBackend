using FinanceTracker.Domain.Models.DTOs;

namespace FinanceTracker.Services.Processings
{
    public interface IDashboardProcessingService
    {
        ValueTask<DashboardSummaryDto> GetDashboardData(Guid userId);
    }
}
