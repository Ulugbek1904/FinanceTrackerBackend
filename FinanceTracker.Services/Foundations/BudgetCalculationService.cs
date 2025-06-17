using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models.DTOs.BudgetDtos;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;

namespace FinanceTracker.Services.Foundations
{
    public class BudgetCalculationService : IBudgetCalculationService
    {
        private readonly ITransactionOrchestration transaction;
        private readonly IBudgetService budgetService;

        public BudgetCalculationService(
            ITransactionOrchestration transaction,
            IBudgetService budgetService
            )
        {
            this.transaction = transaction;
            this.budgetService = budgetService;
        }
        public async ValueTask<BudgetStatsDto> GetBudgetStatsAsync(Guid userId, Guid budgetId)
        {
            var budget = await budgetService.RetrieveBudgetByIdAsync(userId, budgetId);

            if (budget == null)
                throw new AppException("Budget not found");

            var transactions = await transaction.GetTransactionsByBudgetAsync(
                userId, budget.StartDate, budget.EndDate, budget.CategoryId);

            var totalSpent = transactions.Sum(t => t.Amount);

            return new BudgetStatsDto
            {
                LimitAmount = budget.LimitAmount,
                RemainingAmount = budget.LimitAmount - totalSpent,
                TotalSpent = totalSpent
            };
        }
    }
}
