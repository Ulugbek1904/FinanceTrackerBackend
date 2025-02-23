using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Services.Foundations.Interfaces;

namespace FinanceTracker.Services.Foundations
{
    public class CategoryService : ICategoryService
    {
        private readonly IStorageBroker storageBroker;

        public CategoryService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker;
        }

        public ValueTask<Category> CreateCategoryAsync(Category category)
         => this.storageBroker.InsertAsync(category);

        public IQueryable<Category> RetrieveAllCategories()
            => this.storageBroker.SelectAll<Category>();
    }
}
