using FinanceTracker.Domain.Models;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface ICategoryService
    {
        ValueTask<Category> CreateCategoryAsync(Category category);
        IQueryable<Category> RetrieveAllCategories();
    }
}
