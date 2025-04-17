using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface IBudgetService
    {
        Task<IEnumerable<BudgetDto>> RetrieveAllBudgetsAsync(Guid userId);
        ValueTask<BudgetDto?> RetrieveBudgetByIdAsync(Guid userId,Guid budgetId);
        ValueTask<BudgetDto> CreateBudgetAsync(Guid userId, BudgetCreateDto createDto);
        ValueTask<BudgetDto> UpdateBudgetAsync(Guid id, Guid userId, BudgetUpdateDto updateDto);
        ValueTask<bool> DeleteBudgetAsync(Guid userId, Guid budgetId);
    }
}
