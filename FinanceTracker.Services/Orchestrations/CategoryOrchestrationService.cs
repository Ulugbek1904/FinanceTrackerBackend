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

        public IQueryable<Category> RetrieveCategoriesByType(Guid userId, bool isIncome)
        {
            var categries = this.categoryService.RetrieveAllCategories()
                .Where(c => c.IsIncome == isIncome && (c.UserId == userId || c.UserId == null));

            return categries;
        }

        public IQueryable<Category> RetrieveCategoriesByUserId(Guid userId)
        {
            return this.categoryService.RetrieveAllCategories().
                Where(c => c.UserId == userId || c.UserId == null);
        }

        public async ValueTask<Category> UpdateCategoryAsync(Guid userId, Category updatedCategory)
        {
            var existingCategory = await this.categoryService.RetrieveCategoryByIdAsync(updatedCategory.Id);

            if (existingCategory.UserId != userId)
                throw new UnauthorizedAccessException("You can't update this category.");

            existingCategory.Name = updatedCategory.Name;
            existingCategory.IsIncome = updatedCategory.IsIncome;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            return await this.categoryService.UpdateCategoryAsync(existingCategory);
        }

        public async ValueTask<Category> RemoveCategoryByIdAsync(Guid userId, int categoryId)
        {
            var category = await this.categoryService.RetrieveCategoryByIdAsync(categoryId);

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("You can't delete this category.");

            return await this.categoryService.RemoveCategoryAsync(categoryId);
        }

    }
}
