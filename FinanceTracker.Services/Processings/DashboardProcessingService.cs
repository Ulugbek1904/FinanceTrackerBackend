using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations.Interfaces;

namespace FinanceTracker.Services.Processings
{
    public class DashboardProcessingService : IDashboardProcessingService
    {
        private readonly ITransactionService transactionService;

        public DashboardProcessingService(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        public DashboardData GetDashboardData(Guid userId)
        {
            var transactions = this.transactionService
                .RetrieveAllTransactions(userId).ToList();

            return new DashboardData
            {
                CurrentBalance = transactions.Sum(
                    t => t.TransactionType == TransactionType.Income
                    ? t.Amount : -t.Amount),
                MonthlyTrend = CalculateMonthlyTrend(transactions)
            };

        }

        private Dictionary<string, decimal> CalculateMonthlyTrend(List<Transaction> transactions)
        {
            return transactions
                .GroupBy(t => t.TransactionDate.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key, g => g.Sum(t => t.
                TransactionType == TransactionType.Income ? t.Amount : -t.Amount));
        }
    }
}
