using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services.Orchestrations
{
    public class ReportOrchestrationService : IReportOrchestrationService
    {
        private readonly ICategoryService category;
        private readonly ITransactionService transactionService;

        public ReportOrchestrationService(
            ICategoryService category,
            ITransactionService transactionService)
        {
            this.category = category;
            this.transactionService = transactionService;
        }

        public async ValueTask<FinancialReport> GenerateMonthlyReport(Guid userId, int year, int month)
        {
            var transactions = await this.transactionService.RetrieveAllTransactions(userId)
                .Where(t => t.TransactionDate.Year == year && t.TransactionDate.Month == month)
                .ToListAsync();


            var categories = await this.category.RetrieveAllCategories()
                .Where(c => c.UserId == userId || c.UserId ==null).ToListAsync();

            var categorySums = transactions
                .GroupBy(t => t.Category.Id)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(t => t.Amount));

            return new FinancialReport
            {
                Year = year,
                Month = month,
                TotalIncome = transactions.Where(t => t.TransactionType == TransactionType.Income)
                    .Sum(t => t.Amount),
                TotalExpense = transactions.Where(t => t.TransactionType == TransactionType.Expense)
                    .Sum(t => t.Amount),
                Categories = categories.Select(c => new CategorySummary
                {
                    Name = c.Name,
                    Amount = categorySums.ContainsKey(c.Id) ? categorySums[c.Id] : 0
                }).ToList(),
                NetBalance = transactions.Where(t => t.TransactionType == TransactionType.Income)
                    .Sum(t => t.Amount) - transactions.Where(t => t.TransactionType == TransactionType.Expense)
                    .Sum(t => t.Amount)
            };
        }
    }
}
