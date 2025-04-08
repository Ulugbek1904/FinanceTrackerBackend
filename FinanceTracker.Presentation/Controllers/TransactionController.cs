using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
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
                var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userIdentifier == null)
                    return Unauthorized();

                var userId = Guid.Parse(userIdentifier);
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
        [Route("getting-all-transactions")]
        public IActionResult GetAllTransactions()
        {
            try
            {
                var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdentifier == null)
                    return Unauthorized();

                var userId = Guid.Parse(userIdentifier);

                var transactions = orchestration.RetrieveAllTransactions(userId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-transaction-by-id/{id:guid}")]
        public async ValueTask<IActionResult> GetTransactionById(Guid id)
        {
            try
            {
                var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userIdentifier == null)
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
                var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdentifier == null)
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
                var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdentifier == null)
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
    }
}
