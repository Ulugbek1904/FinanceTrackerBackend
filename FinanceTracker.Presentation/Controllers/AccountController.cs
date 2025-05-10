using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTFulSense.Controllers;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Authorize]
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
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var accounts = await accountService
                .GetAccountsByUserId(userId.Value).ToListAsync();

            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async ValueTask<IActionResult> GetAccountByIdAsync([FromRoute] Guid id)
        {
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var account = await accountService.GetAccountByIdAsync(id);

            if (account.UserId != userId)
                throw new ForbiddenAccessException();

            return Ok(account);
        }
        
        [HttpPost("create")]
        public async ValueTask<IActionResult> CreateAccountAsync([FromBody] CreateAccountDto accountDto)
        {

            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException("User is not authorized.");

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
                throw new AppException("Account creation failed.");

            return Created(createdAccount);
        }
        // done
        [HttpPut("{id}/update-balance")]
        public async ValueTask<IActionResult> UpdateAccountAsync(Guid accountId, [FromBody] CreateAccountDto accountDto)
        {
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var existingAccount = await accountService.GetAccountByIdAsync(accountId);

            if (existingAccount.UserId != userId)
                throw new ForbiddenAccessException("You are not allowed to update this account.");

            existingAccount.Balance = accountDto.Balance;
            existingAccount.Name = accountDto.Name;
            existingAccount.Type = accountDto.Type;
            existingAccount.IsPrimary = accountDto.IsPrimary;
            existingAccount.UserId = userId.Value;

            var updated = await accountService.UpdateAccountAsync(existingAccount);

            return Ok(updated);
        }

        [HttpDelete("{accountId}")]
        public async ValueTask<IActionResult> DeleteAccountAsync(Guid accountId)
        {
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var account = await accountService.GetAccountByIdAsync(accountId);

            if (account.UserId != userId)
                throw new ForbiddenAccessException("You are not allowed to delete this account.");

            var deleted = await accountService.DeleteAccountAsync(account);

            return Ok(deleted);
        }

        [HttpPatch("set-primary/{accountId}")]
        public async ValueTask<IActionResult> SetPrimaryAccount(Guid accountId)
        {
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var existingAccount = await accountService.GetAccountByIdAsync(accountId);

            if (existingAccount.UserId != userId)
                throw new ForbiddenAccessException("You are not allowed to set this account as primary.");

            existingAccount.IsPrimary = true;

            await accountService.UpdateAccountAsync(existingAccount);

            return Ok(existingAccount);
        }

        [HttpPatch("{accountId}")]
        public async ValueTask<IActionResult> UpdateAccountBalance(Guid accountId, decimal balance)
        {
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException("User is not authorized.");

            var existingAccount = await accountService.GetAccountByIdAsync(accountId);

            if (existingAccount.UserId != userId)
                throw new ForbiddenAccessException("You are not allowed to update this account.");

            existingAccount.Balance = balance;

            await accountService.UpdateAccountAsync(existingAccount);

            return Ok(existingAccount);
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
