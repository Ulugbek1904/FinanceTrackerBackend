using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services.Processings
{
    public class DashboardProcessingService : IDashboardProcessingService
    {
        private readonly ITransactionService transactionService;

        public DashboardProcessingService(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        public async ValueTask<DashboardSummaryDto> GetDashboardData(Guid userId)
        {
            var transactions = await transactionService
                .RetrieveAllTransactions(userId)
                .Include(t => t.Category)
                .Include(t => t.Account)
                .ToListAsync();

            var totalIncome = transactions
                .Where(t => t.TransactionType == TransactionType.Income)
                .Sum(t => t.Amount);

            var totalExpence = transactions
                .Where(t => t.TransactionType == TransactionType.Expense)
                .Sum(t => t.Amount);

            var netBalance = totalIncome - totalExpence;

            var recentTransactions = transactions
                .OrderByDescending(t => t.TransactionDate)
                .Take(8)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Description = t.Description,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    TransactionType = t.TransactionType,
                    CategoryName = t.Category.Name,
                    AccountName = t.Account.Name
                }).ToList();

            var topCategories = transactions
                .GroupBy(t => t.Category.Name)
                .Select(g => new CategorySummary
                {
                    Name = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(cs => cs.Amount)
                .Take(8).ToList();

            return new DashboardSummaryDto
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpence,
                NetBalance = netBalance,
                RecentTransactions = recentTransactions,
                TopCategories = topCategories
            };

        }
    }
}
