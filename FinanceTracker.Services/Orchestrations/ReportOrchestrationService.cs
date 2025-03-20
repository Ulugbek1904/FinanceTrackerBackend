using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;

namespace FinanceTracker.Services.Orchestrations
{
    public class ReportOrchestrationService : IReportOrchestrationService
    {
        private readonly ICategoryService category;
        private readonly ITransactionService transactionService;
        private readonly IAccountService account;

        public ReportOrchestrationService(
            ICategoryService category,
            ITransactionService transactionService,
            IAccountService account)
        {
            this.category = category;
            this.transactionService = transactionService;
            this.account = account;
        }

        public FinancialReport GenerateMonthlyReport(Guid userId, int year, int month)
        {
            var transactions = this.transactionService.RetrieveAllTransactions(userId)
                .Where(t => t.TransactionDate.Year == year && t.TransactionDate.Month == month)
                .ToList();

            var categories = this.category.RetrieveAllCategories().ToList();

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
                    Amount = transactions.Where(t => t.Category.Id == c.Id)
                        .Sum(t => t.Amount)
                }).ToList(),
                NetBalance = transactions.Where(t => t.TransactionType == TransactionType.Income)
                    .Sum(t => t.Amount) - transactions.Where(t => t.TransactionType == TransactionType.Expense)
                    .Sum(t => t.Amount)
            };
        }
    }
}
