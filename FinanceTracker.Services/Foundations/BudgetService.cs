using AutoMapper;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services.Foundations
{
    public class BudgetService : IBudgetService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IMapper mapper;

        public BudgetService(
            IStorageBroker storageBroker,
            IMapper mapper)
        {
            this.storageBroker = storageBroker;
            this.mapper = mapper;
        }
        public async ValueTask<BudgetDto> CreateBudgetAsync(Guid userId, BudgetCreateDto createDto)
        {
            var budget = this.mapper.Map<Budget>(createDto);
            budget.UserId = userId;

            var category = await this.storageBroker
                .SelectCategoryByIdAsync(createDto.CategoryId);

            if (category is null)
                throw new ArgumentNullException("Category not found");

            budget.Category = category;

            await this.storageBroker.InsertAsync(budget);

            return this.mapper.Map<BudgetDto>(budget);
        }

        public async ValueTask<bool> DeleteBudgetAsync(Guid userId, Guid budgetId)
        {
            var budget = await RetrieveBudgetByIdAsync(userId, budgetId);

            if (budget is null)
                return false;
            
            await this.storageBroker.DeleteAsync(budget);

            return true;
        }

        public async Task<IEnumerable<BudgetDto>> RetrieveAllBudgetsAsync(Guid userId)
        {
            var budgets = await this.storageBroker.SelectAll<Budget>()
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return this.mapper.Map<List<BudgetDto>>(budgets);
        }

        public async ValueTask<BudgetDto?> RetrieveBudgetByIdAsync(Guid userId, Guid budgetId)
        {
            var budget = await this.storageBroker.SelectAll<Budget>()
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .FirstOrDefaultAsync(b => b.Id == budgetId);

            return budget is null
                ? null
                : this.mapper.Map<BudgetDto>(budget);
        }

        public async ValueTask<BudgetDto> UpdateBudgetAsync(Guid id, Guid userId, BudgetUpdateDto updateDto)
        {
            var existingBudget = await storageBroker.SelectAll<Budget>()
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (existingBudget is null)
                throw new ArgumentNullException("Not found");

            mapper.Map(updateDto, existingBudget);
            await storageBroker.UpdateAsync(existingBudget);

            // Reload with related data
            var updatedBudget = await storageBroker.SelectAll<Budget>()
                .Include(b => b.Category)
                .FirstAsync(b => b.Id == id);

            return mapper.Map<BudgetDto>(updatedBudget);
        }
    }
}
