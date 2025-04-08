using FinanceTracker.Domain.Models;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;

namespace FinanceTracker.Services.Orchestrations
{
    public class CategoryOrchestrationService : ICategoryOrchestrationService
    {
        private readonly ICategoryService categoryService;

        public CategoryOrchestrationService(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async ValueTask<Category> AddCategoryAsync(Guid userId, Category category)
        {
            category.UserId = userId;
            category.IsDefault = false;

            return await this.categoryService.CreateCategoryAsync(category);
        }

        public async ValueTask<> RetrieveCategoriesByTypeAsync(Guid userId, bool isIncome)
        {
            var categories = this.categoryService.RetrieveAllCategories()
                .Where(c => c.IsIncome == isIncome && (c.UserId == null || c.UserId == userId));

            return await Task.FromResult(categories.ToList());
        }
    }
}
