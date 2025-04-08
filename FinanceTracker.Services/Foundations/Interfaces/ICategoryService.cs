using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface ICategoryService
    {
        ValueTask<Category> CreateCategoryAsync(Category category);
        IQueryable<Category> RetrieveAllCategories();
        ValueTask<Category> RetrieveCategoryByIdAsync(int categoryId);
        ValueTask<Category> UpdateCategoryAsync(Category category);
        ValueTask<Category> RemoveCategoryAsync(int categoryId);
    }
}
