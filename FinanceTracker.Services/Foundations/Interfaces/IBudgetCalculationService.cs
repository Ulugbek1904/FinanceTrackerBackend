using FinanceTracker.Domain.Models.DTOs.BudgetDtos;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface IBudgetCalculationService
    {
        ValueTask<BudgetStatsDto> GetBudgetStatsAsync(Guid userId, Guid budgetId);
    }
}
