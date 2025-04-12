using CsvHelper;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
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

        public TransactionController(
            ITransactionOrchestration orchestration,
            IAccountService accountService)
        {
            this.orchestration = orchestration;
            this.accountService = accountService;
        }

        [HttpPost]
        [Route("add")]
        public async ValueTask<IActionResult> CreateTransaction([FromBody] TransactionCreateDto transactionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized();

                var account = await accountService.GetAccountByIdAsync(transactionDto.AccountId);

                if (account.UserId != userId)
                    return BadRequest("The account does not belong to you");

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

                var createdTransaction = await orchestration.AddTransactionAsync(transaction);

                return CreatedAtAction(nameof(CreateTransaction), createdTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("transactions")]
        public IActionResult GetAllTransactions()
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized();

                var transactions = orchestration.RetrieveAllTransactions(userId.Value);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("transaction-by-id/{id:guid}")]
        public async ValueTask<IActionResult> GetTransactionById(Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized();

                var transaction = await orchestration.RetrieveTransactionByIdAsync(id);

                return Ok(transaction);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }
        }


        [HttpPut("update-transaction/{id}")]
        public async ValueTask<IActionResult> UpdateTransaction(
            Guid transactionId, [FromBody] TransactionCreateDto transactionDto)
        {
            if(!ModelState.IsValid)
                return BadRequest();
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized();

                var existingTransaction = await 
                    orchestration.RetrieveTransactionByIdAsync(transactionId);

                existingTransaction.Amount = transactionDto.Amount;
                existingTransaction.TransactionDate = transactionDto.TransactionDate;
                existingTransaction.TransactionType = transactionDto.TransactionType;
                existingTransaction.CategoryId = transactionDto.CategoryId;
                existingTransaction.AccountId = transactionDto.AccountId;
                existingTransaction.Description = transactionDto.Description;

                var updatedTransaction = await orchestration.ModifyTransactionAsync(existingTransaction);

                return Ok(updatedTransaction);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("delete-transaction/{id}")]
        public async ValueTask<IActionResult> DeleteTransaction(Guid transactionId)
        {
            try
            {
                var userId = GetUserId();

                if (userId == null)
                    return Unauthorized();

                var transaction = await 
                    orchestration.RetrieveTransactionByIdAsync(transactionId);

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
                    Id = t.Id,
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

        private Guid? GetUserId()
        {
            var userId = User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId);
        }
    }
}
