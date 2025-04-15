using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTFulSense.Controllers;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : RESTFulController
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpGet]
        public async ValueTask<IActionResult> GetAccountsAsync()
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var accounts = await accountService
                    .GetAccountsByUserId(userId.Value).ToListAsync();

                return Ok(accounts);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async ValueTask<IActionResult> GetAccountByIdAsync([FromRoute] Guid id)
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var account = await accountService.GetAccountByIdAsync(id);

                if (account is null)
                    return NotFound();

                if (account.UserId != userId)
                    return Forbid();

                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("create")]
        public async ValueTask<IActionResult> CreateAccountAsync([FromBody] CreateAccountDto accountDto)
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var account = new Account
                {
                    Balance = accountDto.Balance,
                    Name = accountDto.Name,
                    Type = accountDto.Type,
                    IsPrimary = accountDto.IsPrimary,
                    UserId = userId.Value
                };

                var createdAccount = await accountService.CreateAccountAsync(account);

                if (createdAccount is null)
                    return BadRequest("Account creation failed.");

                return CreatedAtAction(nameof(GetAccountByIdAsync),
                    new { id = createdAccount.Id }, createdAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}/update-balance")]
        public async ValueTask<IActionResult> UpdateAccountAsync(Guid categoryId, [FromBody] CreateAccountDto accountDto)
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var existingAccount = await accountService.GetAccountByIdAsync(categoryId);

                if (existingAccount is null)
                    return NotFound();

                if (existingAccount.UserId != userId)
                    return Forbid();

                existingAccount.Balance = accountDto.Balance;
                existingAccount.Name = accountDto.Name;
                existingAccount.Type = accountDto.Type;
                existingAccount.IsPrimary = accountDto.IsPrimary;
                existingAccount.UserId = userId.Value;

                var updated = await accountService.UpdateAccountAsync(existingAccount);

                if (updated is null) return BadRequest(updated);

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{accountId}")]
        public async ValueTask<IActionResult> DeleteAccountAsync(Guid accountId)
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var account = await accountService.GetAccountByIdAsync(accountId);

                if (account is null)
                    return NotFound();

                if(account.UserId != userId)
                    return Forbid();

                var deleted = await accountService.DeleteAccountAsync(account);
                if (deleted is null) return BadRequest(deleted);

                return Ok(deleted);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }
        }

        [HttpPatch("{accountId}")]
        public async ValueTask<IActionResult> SetPrimaryAccount(Guid accountId)
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var existingAccount = await accountService.GetAccountByIdAsync(accountId);

                if(existingAccount is null)
                    return NotFound();

                if(existingAccount.UserId != userId)
                    return Forbid();

                existingAccount.IsPrimary = true;

                await accountService.UpdateAccountAsync(existingAccount);

                return Ok(existingAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{accountId}")]
        public async ValueTask<IActionResult> UpdateAccountBalance(Guid accountId, decimal balance)
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var existingAccount = await accountService.GetAccountByIdAsync(accountId);

                if (existingAccount is null)
                    return NotFound();

                if (existingAccount.UserId != userId)
                    return Forbid();

                existingAccount.Balance = balance;

                await accountService.UpdateAccountAsync(existingAccount);

                return Ok(existingAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        private Guid? GetUserId()
        {
            var userClaims = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userClaims is null)
                return null;

            if (Guid.TryParse(userClaims.Value, out Guid userId))
                return userId;
            return null;
        }
    }
}
