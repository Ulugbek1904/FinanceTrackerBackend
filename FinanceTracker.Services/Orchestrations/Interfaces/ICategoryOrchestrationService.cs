using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Orchestrations.Interfaces
{
    public interface ICategoryOrchestrationService
    {
        ValueTask<Category> AddCategoryAsync(Guid userId, Category category);
        IQueryable<Category> RetrieveCategoriesByUserId(Guid userId);
        IQueryable<Category> RetrieveCategoriesByType(Guid userId, bool isIncome);
        ValueTask<Category> RemoveCategoryByIdAsync(Guid userId, int categoryId);
        ValueTask<Category> UpdateCategoryAsync(Guid userId, Category updatedCategory);
    }

}
