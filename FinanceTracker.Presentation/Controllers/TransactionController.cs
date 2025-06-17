using CsvHelper;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Domain.Models.DTOs.PageDto;
using FinanceTracker.Domain.Models.DTOs.TransactionDtos;
using FinanceTracker.Services.Foundations;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTFulSense.Controllers;
using System.Globalization;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/transaction")]
    [Authorize]
    public class TransactionController : RESTFulController
    {
        private readonly ITransactionOrchestration orchestration;
        private readonly IAccountService accountService;
        private readonly ITransactionService transactionService;

        public TransactionController(
            ITransactionOrchestration orchestration,
            IAccountService accountService,
            ITransactionService transactionService)
        {
            this.orchestration = orchestration;
            this.accountService = accountService;
            this.transactionService = transactionService;
        }

        [HttpPost]
        [Route("add")]
        public async ValueTask<IActionResult> CreateTransaction([FromBody] TransactionCreateDto transactionDto)
        {
            var userId = GetUserId();

            if (userId == null)
                throw new UnauthorizedAccessException();

            var account = await accountService.GetAccountByIdAsync(transactionDto.AccountId);

            if (account.UserId != userId)
                throw new ForbiddenAccessException("The account does not belong to you");

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Description = transactionDto.Description,
                Amount = transactionDto.Amount,
                TransactionDate = transactionDto.TransactionDate,
                TransactionType = transactionDto.TransactionType,
                CategoryId = transactionDto.CategoryId,
                AccountId = transactionDto.AccountId,
            };

            var createdTransaction = await orchestration.AddTransactionAsync(userId, transaction);

            return CreatedAtAction(nameof(CreateTransaction), createdTransaction);
        }

        [HttpGet]
        [Route("all")]
        public async ValueTask<IActionResult> GetAllTransactions([FromQuery] TransactionQueryDto queryDto)
        {
            var userId = GetUserId();

            if (userId == null)
                throw new UnauthorizedAccessException();

            var transactions = await orchestration.RetrieveTransactionsWithQueryAsync(userId.Value, queryDto);
            return Ok(transactions);
        }


        [HttpGet("transaction-by-id/{id:guid}")]
        public async ValueTask<IActionResult> GetTransactionById(Guid id)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var transaction = await transactionService.RetrieveTransactionByIdAsync(id);

            return Ok(transaction);
        }


        [HttpPut("update-transaction/{transactionId}")]
        public async ValueTask<IActionResult> UpdateTransaction(
            Guid transactionId, [FromBody] TransactionCreateDto transactionDto)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var existingTransaction = await transactionService.RetrieveTransactionByIdAsync(transactionId);
            if (existingTransaction == null)
                return NotFound("Transaction not found.");

            var updatedTransaction = new Transaction
            {
                Id = transactionId,
                Description = transactionDto.Description,
                Amount = transactionDto.Amount,
                TransactionDate = transactionDto.TransactionDate,
                TransactionType = transactionDto.TransactionType,
                CategoryId = transactionDto.CategoryId,
                AccountId = transactionDto.AccountId
            };

            var result = await orchestration.ModifyTransactionAsync(updatedTransaction);

            return Ok(result);
        }

        [HttpDelete("delete-transaction/{transactionId}")]
        public async ValueTask<IActionResult> DeleteTransaction(Guid transactionId)
        {
            try
            {   
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized();

                var transaction = await
                    transactionService.RetrieveTransactionByIdAsync(transactionId);

                if (transaction is null)
                    return NotFound();

                await orchestration.RemoveTransactionByIdAsync(transactionId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("export")]
        public async ValueTask<IActionResult> ExportCsvFile()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var transactions = await this.orchestration
                .RetrieveAllTransactions(userId.Value)
                .Select(t => new TransactionDto
                {
                    Description = t.Description,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    TransactionType = t.TransactionType,
                    AccountName = t.Account.Name,
                    CategoryName = t.Category.Name,
                })
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(transactions);
            writer.Flush();

            var fileName = $"Transactions-{DateTime.UtcNow:yyyyMMddHH}.csv";

            return File(memoryStream.ToArray(),"text/csv", fileName);
        }

        [HttpGet("by-budget")]
        public async ValueTask<IActionResult> GetTransactionsByBudget(
            [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int categoryId)
        {
            var userIdentifier = GetUserId();
            if (userIdentifier is null)
                throw new UnauthorizedAccessException();
            var userId = userIdentifier.Value;

            var transactions = await orchestration.GetTransactionsByBudgetAsync(userId, startDate, endDate, categoryId);
            return Ok(transactions);
        }

        private Guid? GetUserId()
        {
            var userId = User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId);
        }
    }
}
