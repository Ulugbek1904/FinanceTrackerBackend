using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTFulSense.Controllers;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/budgets")]
    public class BudgetController : RESTFulController
    {
        private readonly IBudgetService budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            this.budgetService = budgetService;
        }

        [HttpGet]
        public async ValueTask<IActionResult> GetBudgets()
        {
            var userIdentifier = GetUserId();

            if (userIdentifier is null)
                return Unauthorized();

            var userId = userIdentifier.Value;

            var budgets = await budgetService.RetrieveAllBudgetsAsync(userId);

            return Ok(budgets);
        }

        [HttpGet("{budgetId}")]
        public async ValueTask<IActionResult> GetBudgetById(Guid budgetId)
        {
            var userIdentifier = GetUserId();

            if (userIdentifier is null)
                return Unauthorized();

            var userId = userIdentifier.Value;

            var budget = await budgetService
                .RetrieveBudgetByIdAsync(userId, budgetId);

            return budget is null
                ? NotFound()
                : Ok(budget);
        }

        [HttpPost]
        public async ValueTask<IActionResult> CreateBudget([FromBody] BudgetCreateDto budget)
        {
            var userIdentifier = GetUserId();

            if (userIdentifier is null)
                return Unauthorized();

            var userId = userIdentifier.Value;

            var createdBudget = await budgetService
                .CreateBudgetAsync(userId, budget);

            return Created(createdBudget);
        }

        [HttpPut("{budgetId}")]
        public async ValueTask<IActionResult> UpdateBudget(Guid budgetId, [FromBody] BudgetUpdateDto budget)
        {
            var userIdentifier = GetUserId();

            if (userIdentifier is null)
                return Unauthorized();

            var userId = userIdentifier.Value;

            var updatedBudget = await budgetService
                .UpdateBudgetAsync(budgetId, userId, budget);

            return updatedBudget is null
                ? NotFound()
                : Ok(updatedBudget);
        }

        [HttpDelete("{budgetId}")]
        public async ValueTask<IActionResult> DeleteBudget(Guid budgetId)
        {
            var userIdentifier = GetUserId();

            if (userIdentifier is null)
                return Unauthorized();

            var userId = userIdentifier.Value;

            var isDeleted = await budgetService
                .DeleteBudgetAsync(userId, budgetId);

            return isDeleted
                ? NoContent()
                : NotFound();
        }
        private Guid? GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userId == null)
                return null;

            return Guid.Parse(userId.Value) != Guid.Empty
                ? Guid.Parse(userId.Value)
                : null;
        }
    }
}
