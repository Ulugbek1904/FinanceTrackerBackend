using AutoMapper;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs.PageDto;
using FinanceTracker.Domain.Models.DTOs.TransactionDtos;
using FinanceTracker.Infrastructure.Brokers.Storages;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinanceTracker.Services.Orchestrations
{
    public class TransactionOrchestration : ITransactionOrchestration
    {
        private readonly ICategoryService categoryService;
        private readonly IAccountService accountService;
        private readonly ITransactionService transactionService;
        private readonly IMapper mapper;
        private readonly IBudgetService budgetService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly StorageBroker storageBroker;

        public TransactionOrchestration(
            ICategoryService categoryService,
            IAccountService accountService,
            ITransactionService transactionService,
            IMapper mapper,
            IBudgetService budgetService,
            IHttpContextAccessor httpContextAccessor,
            StorageBroker storageBroker
            )
        {
            this.categoryService = categoryService;
            this.accountService = accountService;
            this.transactionService = transactionService;
            this.mapper = mapper;
            this.budgetService = budgetService;
            this.httpContextAccessor = httpContextAccessor;
            this.storageBroker = storageBroker;
        }

        public async ValueTask<TransactionDto> AddTransactionAsync(Guid? userId, Transaction transaction)
        {
            var existingCategory = this.categoryService
                .RetrieveAllCategories()
                .FirstOrDefault(c => c.Id == transaction.CategoryId);

            if (existingCategory is null)
                throw new CategoryNotFoundException("Category not found.");

            var account = await this.accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account is null)
                throw new AccountNullException("Account not found.");

            var budgets = await budgetService.RetrieveAllBudgetsAsync(userId);

            bool isIncome = existingCategory.IsIncome;

            if(transaction.TransactionType != TransactionType.Expense 
                && transaction.TransactionType != TransactionType.Income)
            {
                throw new AppException("Invalid transaction type. Must be Income(0) or Expense(1)");
            }

            if(transaction.TransactionType == TransactionType.Expense && isIncome||
                transaction.TransactionType == TransactionType.Income && !isIncome)
            {
                throw new AppException("Transaction type doesn't match category type. " +
                    "Income transactions must use income categories and expense transactions must use expense categories");
            }

            if (transaction.TransactionType == TransactionType.Expense)
            {
                var budget = budgets.FirstOrDefault(b => b.CategoryId == transaction.CategoryId);
                if (budget is not null && transaction.TransactionDate >= budget.StartDate &&
                    transaction.TransactionDate <= budget.EndDate)
                {
                    var totalExpenses = await transactionService.GetTotalExpensesByCategoryAsync(
                        userId,
                        transaction?.CategoryId,
                        budget.StartDate,
                        budget.EndDate);

                    var newTotal = totalExpenses + transaction.Amount;

                    if (newTotal > budget.LimitAmount)
                    {
                        throw new AppException($"Transaction exceeds budget limit for category {existingCategory.Name}. " +
                            $"Budget limit: {budget.LimitAmount}, Current expenses: {totalExpenses}, Attempted: {transaction.Amount}");
                    }
                }


                if (account.Balance < transaction.Amount)
                    throw new AppException("bu akkauntingda yetarlicha pul yo'q");
                account.Balance -= transaction.Amount;
            }
            else if (transaction.TransactionType == TransactionType.Income)
            {
                account.Balance += transaction.Amount;
            }

            await this.accountService.UpdateAccountBalanceAsync(account.Id, account.Balance);

            var createdTransaction =  await transactionService.CreateTransactionAsync(transaction);

            return mapper.Map<TransactionDto>(createdTransaction);
        }

        public async ValueTask<Transaction> ModifyTransactionAsync(Transaction transaction)
        {
            var user = this.httpContextAccessor.HttpContext?.User;
            var userId = Guid.Parse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var existingTransaction = await this.transactionService.RetrieveTransactionByIdAsync(transaction.Id);
            if (existingTransaction == null)
                throw new InvalidOperationException("Transaction not found.");

            Console.WriteLine($"Eski tranzaksiya: ID={existingTransaction.Id}, Amount={existingTransaction.Amount}, Type={existingTransaction.TransactionType}, AccountId={existingTransaction.AccountId}");
            Console.WriteLine($"Yangi tranzaksiya: ID={transaction.Id}, Amount={transaction.Amount}, Type={transaction.TransactionType}, AccountId={transaction.AccountId}");

            var currentAccount = await this.accountService.GetAccountByIdAsync(existingTransaction.AccountId);
            if (currentAccount == null)
                throw new InvalidOperationException("Current account not found.");

            var newAccount = existingTransaction.AccountId == transaction.AccountId
                ? currentAccount
                : await this.accountService.GetAccountByIdAsync(transaction.AccountId);
            if (newAccount == null)
                throw new InvalidOperationException("New account not found.");

            var existingCategory = this.categoryService
                .RetrieveAllCategories()
                .FirstOrDefault(c => c.Id == transaction.CategoryId);
            if (existingCategory == null)
                throw new CategoryNotFoundException("Category not found.");

            var budgets = await budgetService.RetrieveAllBudgetsAsync(userId);
            bool isIncome = existingCategory.IsIncome;

            if (transaction.TransactionType != TransactionType.Expense && transaction.TransactionType != TransactionType.Income)
                throw new AppException("Invalid transaction type. Must be Income(0) or Expense(1)");

            if (transaction.TransactionType == TransactionType.Expense && isIncome ||
                transaction.TransactionType == TransactionType.Income && !isIncome)
                throw new AppException("Transaction type doesn't match category type. " +
                    "Income transactions must use income categories and expense transactions must use expense categories");

            if (existingTransaction.TransactionType == TransactionType.Expense)
            {
                currentAccount.Balance += existingTransaction.Amount; 
                Console.WriteLine($"Eski chiqim qaytarildi: AccountId={currentAccount.Id}, Balans={currentAccount.Balance}");
            }
            else if (existingTransaction.TransactionType == TransactionType.Income)
            {
                currentAccount.Balance -= existingTransaction.Amount;
                Console.WriteLine($"Eski kirim olib tashlandi: AccountId={currentAccount.Id}, Balans={currentAccount.Balance}");
            }

            if (transaction.TransactionType == TransactionType.Expense)
            {
                var budget = budgets.FirstOrDefault(b => b.CategoryId == transaction.CategoryId);
                if (budget != null && transaction.TransactionDate >= budget.StartDate &&
                    transaction.TransactionDate <= budget.EndDate)
                {
                    var totalExpenses = await transactionService.GetTotalExpensesByCategoryAsync(
                        userId, transaction.CategoryId, budget.StartDate, budget.EndDate);

                    totalExpenses -= (existingTransaction.TransactionType == TransactionType.Expense && existingTransaction.AccountId == transaction.AccountId)
                        ? existingTransaction.Amount
                        : 0;

                    var newTotal = totalExpenses + transaction.Amount;
                    if (newTotal > budget.LimitAmount)
                        throw new AppException($"Transaction exceeds budget limit for category {existingCategory.Name}. " +
                            $"Budget limit: {budget.LimitAmount}, Current expenses: {totalExpenses}, Attempted: {transaction.Amount}");
                }

                if (newAccount.Balance < transaction.Amount)
                    throw new AppException($"Yangi hisobda (AccountId={newAccount.Id}) yetarlicha pul yo‘q. Balans: {newAccount.Balance}, Talab qilingan: {transaction.Amount}");

                newAccount.Balance -= transaction.Amount; 
                Console.WriteLine($"Yangi chiqim qo‘llanildi: AccountId={newAccount.Id}, Balans={newAccount.Balance}");
            }
            else if (transaction.TransactionType == TransactionType.Income)
            {
                newAccount.Balance += transaction.Amount; 
                Console.WriteLine($"Yangi kirim qo‘llanildi: AccountId={newAccount.Id}, Balans={newAccount.Balance}");
            }

            using (var dbContextTransaction = await this.storageBroker.Database.BeginTransactionAsync())
            {
                try
                {
                    if (existingTransaction.AccountId != transaction.AccountId)
                    {
                        await this.accountService.UpdateAccountBalanceAsync(currentAccount.Id, currentAccount.Balance);
                    }

                    await this.accountService.UpdateAccountBalanceAsync(newAccount.Id, newAccount.Balance);

                    var updatedTransaction = await transactionService.ModifyTransactionAsync(transaction);

                    await dbContextTransaction.CommitAsync();
                    return updatedTransaction;
                }
                catch
                {
                    await dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async ValueTask<Transaction> RemoveTransactionByIdAsync(Guid transactionId)
        {
            var transaction = await this.transactionService.RetrieveTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new InvalidOperationException("Transaction not found.");

            var account = await this.accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account == null)
                throw new InvalidOperationException("Account not found.");

            if (transaction.TransactionType == TransactionType.Expense)
            {
                account.Balance += transaction.Amount;
            }
            else if (transaction.TransactionType == TransactionType.Income)
            {
                account.Balance -= transaction.Amount;
            }

            await this.accountService.UpdateAccountBalanceAsync(account.Id, account.Balance);
            return await transactionService.RemoveTransactionByIdAsync(transactionId);
        }

        public async ValueTask<PagedResult<TransactionDto>> RetrieveTransactionsWithQueryAsync
           (Guid userId, TransactionQueryDto queryDto)
        {
            int pageNumber = queryDto.PageNumber <= 0 ? 1 : queryDto.PageNumber;
            int pageSize = queryDto.PageSize <= 0 ? 10 : queryDto.PageSize;
            queryDto.PageSize = pageSize;
            queryDto.PageNumber = pageNumber;

            var query = this.transactionService.RetrieveAllTransactions(userId);

            query = ApplyFiltering(query, queryDto);
            query = ApplySorting(query, queryDto);

            if (!string.IsNullOrEmpty(queryDto.Search))
            {
                query = query.Where(t => t.Description.Contains(queryDto.Search));
            }

            var totalCount = await query.CountAsync();

            var transactions = await query
                .Skip((queryDto.PageNumber - 1) * queryDto.PageSize)
                .Take(queryDto.PageSize)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    TransactionType = t.TransactionType,
                    AccountName = t.Account.Name,
                    CategoryName = t.Category.Name
                })
                .ToListAsync();

            return new PagedResult<TransactionDto>
            {
                Items = transactions,
                TotalCount = totalCount,
                PageNumber = queryDto.PageNumber,
                PageSize = queryDto.PageSize
            };
        }

        public IQueryable<Transaction> RetrieveAllTransactions(Guid? userId)
        {
            if (userId == Guid.Empty)
                throw new AppException("Invalid user ID.");

            var transactions = transactionService
                .RetrieveAllTransactions(userId)
                .AsNoTracking();

            if (!transactions.Any())
                throw new TransactionNotFoundException("Transaction not found");

            return transactions;
        }

        private IQueryable<Transaction> ApplySorting(IQueryable<Transaction> transactions, TransactionQueryDto queryDto)
        {
            if (string.IsNullOrEmpty(queryDto.SortBy))
                return queryDto.IsDescending
                    ? transactions.OrderByDescending(t => t.TransactionDate)
                    : transactions.OrderBy(t => t.TransactionDate);

            var sortProperty = queryDto.SortBy.ToLower();

            return sortProperty switch
            {
                "amount" => queryDto.IsDescending
                    ? transactions.OrderByDescending(t => t.Amount)
                    : transactions.OrderBy(t => t.Amount),
                "date" => queryDto.IsDescending
                    ? transactions.OrderByDescending(t => t.TransactionDate)
                    : transactions.OrderBy(t => t.TransactionDate),
                "category" => queryDto.IsDescending
                    ? transactions.OrderByDescending(t => t.CategoryId)
                    : transactions.OrderBy(t => t.CategoryId),
                "account" => queryDto.IsDescending
                    ? transactions.OrderByDescending(t => t.AccountId)
                    : transactions.OrderBy(t => t.AccountId),
                _ => queryDto.IsDescending
                    ? transactions.OrderByDescending(t => t.TransactionDate)
                    : transactions.OrderBy(t => t.TransactionDate)
            };
        }

        private IQueryable<Transaction> ApplyFiltering(IQueryable<Transaction> transactions, TransactionQueryDto dto)
        {
            if (dto.AccountId.HasValue)
                transactions = transactions.Where(t => t.AccountId == dto.AccountId);
            
            if (dto.CategoryId.HasValue)
                transactions = transactions.Where(t => t.CategoryId == dto.CategoryId);
            
            if (dto.IsIncome.HasValue)
                transactions = transactions.Where(t => t.TransactionType == 
                (dto.IsIncome.Value ? TransactionType.Income : TransactionType.Expense));

            if(dto.StartDate.HasValue && dto.EndDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate >= dto.StartDate && t.TransactionDate <= dto.EndDate);

            return transactions;
        }

        public async ValueTask<IEnumerable<TransactionDto>> GetTransactionsByBudgetAsync(
                Guid userId, DateTime startDate, DateTime endDate, int categoryId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            var query = RetrieveAllTransactions(userId)
                .Where(t => t.CategoryId == categoryId)
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Description = t.Description,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    TransactionType = t.TransactionType,
                    CategoryName = t.Category.Name, 
                    AccountName = t.Account.Name           
                });

            var result = await query.ToListAsync();

            if (!result.Any())
                return [];

            return result;
        }
    }

}
