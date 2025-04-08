using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Orchestrations.Interfaces
{
    public interface ICategoryOrchestrationService
    {
        ValueTask<Category> AddCategoryAsync(Guid userId, Category category);
        ValueTask<IEnumerable<Category>> RetrieveCategoriesByUserIdAsync(Guid userId);
        ValueTask<IEnumerable<Category>> RetrieveCategoriesByTypeAsync(Guid userId, bool isIncome);
    }

}
