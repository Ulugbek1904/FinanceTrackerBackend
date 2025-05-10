using AutoMapper;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs.PageDto;
using FinanceTracker.Domain.Models.DTOs.TransactionDtos;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FinanceTracker.Services.Orchestrations
{
    public class TransactionOrchestration : ITransactionOrchestration
    {
        private readonly ICategoryService categoryService;
        private readonly IAccountService accountService;
        private readonly ITransactionService transactionService;
        private readonly IMapper mapper;

        public TransactionOrchestration(
            ICategoryService categoryService,
            IAccountService accountService,
            ITransactionService transactionService,
            IMapper mapper)
        {
            this.categoryService = categoryService;
            this.accountService = accountService;
            this.transactionService = transactionService;
            this.mapper = mapper;
        }

        public async ValueTask<TransactionDto> AddTransactionAsync(Transaction transaction)
        {
            // Validating category existence
            var existingCategory = this.categoryService
                .RetrieveAllCategories()
                .FirstOrDefault(c => c.Id == transaction.CategoryId);

            if (existingCategory is null)
                throw new InvalidOperationException("Category not found.");

            // Validating account existence
            var account = await this.accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account is null)
                throw new InvalidOperationException("Account not found.");

            bool isIncome = existingCategory.IsIncome;

            if(transaction.TransactionType != TransactionType.Expense 
                && transaction.TransactionType != TransactionType.Income)
            {
                throw new InvalidOperationException("Invalid transaction type. Must be Income(0) or Expense(1)");
            }

            if(transaction.TransactionType == TransactionType.Expense && isIncome ||
                transaction.TransactionType == TransactionType.Income && !isIncome)
            {
                throw new InvalidOperationException("Transaction type doesn't match category type. " +
                    "Income transactions must use income categories and expense transactions must use expense categories");
            }

            if (transaction.TransactionType == TransactionType.Expense)
            {
                if (account.Balance < transaction.Amount)
                    throw new InvalidOperationException("Insufficient funds.");
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
            var existingTransaction = await this.transactionService.RetrieveTransactionByIdAsync(transaction.Id);
            if (existingTransaction == null)
                throw new InvalidOperationException("Transaction not found.");

            var account = await this.accountService.GetAccountByIdAsync(transaction.AccountId);
            if (account == null)
                throw new InvalidOperationException("Account not found.");

            if (existingTransaction.TransactionType == TransactionType.Expense)
            {
                account.Balance += existingTransaction.Amount; 
            }
            else if (existingTransaction.TransactionType == TransactionType.Income)
            {
                account.Balance -= existingTransaction.Amount; 
            }

            if (transaction.TransactionType == TransactionType.Expense)
            {
                if (account.Balance < transaction.Amount)
                    throw new InvalidOperationException("Insufficient funds.");
                account.Balance -= transaction.Amount;
            }
            else if (transaction.TransactionType == TransactionType.Income)
            {
                account.Balance += transaction.Amount;
            }

            await this.accountService.UpdateAccountBalanceAsync(account.Id, account.Balance);
            return await transactionService.ModifyTransactionAsync(transaction);
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

        public async ValueTask<Transaction> RetrieveTransactionByIdAsync(Guid transactionId)
        {
            if (transactionId == Guid.Empty)
                throw new ArgumentException("Invalid transaction ID.", nameof(transactionId));

            var transaction = await transactionService.RetrieveTransactionByIdAsync(transactionId);

            if (transaction is null)
                throw new TransactionNotFoundException("Transaction not found");

            return transaction;
        }

        public async ValueTask<PagedResult<TransactionDto>> RetrieveTransactionsWithQueryAsync
            (Guid userId, TransactionQueryDto queryDto)
        {
            try
            {
                int pageNumber = queryDto.PageNumber <= 0 ? 1 : queryDto.PageNumber;
                int pageSize = queryDto.PageSize <= 0 ? 10 : queryDto.PageSize;
                queryDto.PageSize = pageSize;
                queryDto.PageNumber = pageNumber;

                var query = this.transactionService.RetrieveAllTransactions(userId);

                query = ApplyFiltering(query, queryDto);
                query = ApplySorting(query, queryDto);

                var totalCount = await query.CountAsync();

                var transactions = await query
                    .Skip((queryDto.PageNumber - 1) * queryDto.PageSize)
                    .Take(queryDto.PageSize)
                    .Select(t => new TransactionDto
                    {
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
            catch (Exception ex)
            {
                throw new TransactionNotFoundException("Transaction not found.", ex);
            }
        }

        public IQueryable<Transaction> RetrieveAllTransactions(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            var transactions = transactionService.RetrieveAllTransactions(userId);

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
    }

}
