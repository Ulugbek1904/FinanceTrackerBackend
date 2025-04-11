using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Brokers.Logging;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Services.Foundations.Interfaces;
using Serilog;

namespace FinanceTracker.Services.Foundations
{
    public class CategoryService : ICategoryService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker logging;
        private readonly ILogger logger;

        public CategoryService(
            IStorageBroker storageBroker,
            ILoggingBroker logging)
        {
            this.storageBroker = storageBroker;
            this.logging = logging;
        }


        public async ValueTask<Category> CreateCategoryAsync(Category category)
        {
            try
            {
                logging.LogInformation("Category creation initiated");

                if (category == null)
                    throw new ArgumentNullException(nameof(category));

                category.CreatedAt = DateTime.UtcNow;
                return await this.storageBroker.InsertAsync(category);
            }
            catch (Exception ex)
            {
                logging.LogError("Error Creating category", ex);
                throw new CategoryServiceException("Error creating category", ex);
            }
        }

        public IQueryable<Category> RetrieveAllCategories()
        {
            try
            {
                logging.LogInformation("Category retrieval initiated");
                return this.storageBroker.SelectAll<Category>();
            }
            catch (Exception ex)
            {
                logging.LogError("Error retrieving categories", ex);
                throw new CategoryServiceException("Error retrieving categories", ex);
            }
        }

        public async ValueTask<Category> RetrieveCategoryByIdAsync(int categoryId)
        {
            try
            {
                logging.LogInformation($"Retrieving category with ID: {categoryId}");

                if (categoryId == 0)
                    throw new ArgumentNullException(nameof(categoryId));

                var category = await this.storageBroker.SelectCategoryByIdAsync(categoryId);

                if(category is null)
                    throw new CategoryNotFoundException($"Not found category with ID : {categoryId}");

                return category;
            }
            catch (CategoryNotFoundException ex)
            {
                logging.LogError($"Category not found: {ex.Message}",ex);
                throw; 
            }
            catch (Exception ex)
            {
                logging.LogError($"Error retrieving category with ID: {categoryId}", ex);
                throw new CategoryServiceException("Error retrieving category", ex);
            }
        }

        public async ValueTask<Category> UpdateCategoryAsync(Category category)
        {
            try
            {
                logging.LogInformation($"Updating category with ID: {category.Id}");

                if (category == null)
                    throw new ArgumentNullException(nameof(category));

                var categoryId = category.Id;

                var dbCategory = await
                    this.storageBroker.SelectCategoryByIdAsync(categoryId);

                category.UpdatedAt = DateTime.UtcNow;

                return await this.storageBroker.UpdateAsync(category);
            }
            catch (Exception ex)
            {
                logging.LogError("Error updating category", ex);
                throw new CategoryServiceException("Error updating category", ex);
            }
        }

        public async ValueTask<Category> RemoveCategoryAsync(int categoryId)
        {
            try
            {
                logging.LogInformation($"Removing category with ID: {categoryId}");
                if (categoryId == 0)
                    throw new ArgumentNullException(nameof(categoryId));

                var category = await
                    this.storageBroker.SelectCategoryByIdAsync(categoryId);

                return await this.storageBroker.DeleteAsync(category);
            }
            catch (Exception ex)
            {
                logging.LogError($"Error removing category with ID: {categoryId}", ex);
                throw new CategoryServiceException("Error removing category", ex);
            }

        }
    }
}
