using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Domain.Models.DTOs.ReportDtos;
using FinanceTracker.Domain.Models.DTOs.TransactionDtos;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services.Processings;

public class DashboardProcessingService : IDashboardProcessingService
{
    private readonly ITransactionService transactionService;
    private readonly IAccountService accountService;

    public DashboardProcessingService(
        ITransactionService transactionService,
        IAccountService accountService)
    {
        this.transactionService = transactionService;
        this.accountService = accountService;
    }

    public async ValueTask<DashboardSummaryDto> GetDashboardData(Guid userId)
    {
        var transactions = await transactionService
            .RetrieveAllTransactions(userId)
            .Include(t => t.Category)
            .Include(t => t.Account)
            .AsNoTracking()
            .ToListAsync();

        var accountsBalance = this.accountService.GetAccountsByUserId(userId).Select(t => t.Balance).ToList();

        var totalIncome = transactions
            .Where(t => t.TransactionType == TransactionType.Income)
            .Sum(t => t.Amount);

        var totalExpence = transactions
            .Where(t => t.TransactionType == TransactionType.Expense)
            .Sum(t => t.Amount);

        var netBalance = accountsBalance.Sum();

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

        var monthlySummary = transactions.Count() > 0
            ? transactions.GroupBy(t => t.TransactionDate.ToString("yyyy-MM"))
            .Select(g => new MonthlySummaryDto
            {
                Month = g.Key,
                TotalIncome = g.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount),
                TotalExpense = g.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount)
            })
            .OrderBy(m => m.Month)
            .ToList() : new List<MonthlySummaryDto>();


        var accounts = this.accountService.GetAccountsByUserId(userId);
        var accountBalanceDict = accounts.ToDictionary(a => a.Name, a => a.Balance);
        var accountSummary = transactions.GroupBy(t => t.Account.Name)
            .Select(g => new AccountSummaryDto
            {
                AccountName = g.Key,
                TotalBalance = accountBalanceDict.GetValueOrDefault(g.Key,0)
            })
            .OrderByDescending(a => a.TotalBalance)
            .ToList();

        return new DashboardSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpence,
            NetBalance = netBalance,
            MonthlySummary = monthlySummary,
            AccountSummaries = accountSummary,
            RecentTransactions = recentTransactions,
            TopCategories = topCategories
        };

    }
}
