using FinanceTracker.Domain.Models.DTOs.BudgetDtos;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTFulSense.Controllers;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/budget")]
    [Authorize]
    public class BudgetController : RESTFulController
    {
        private readonly IBudgetService budgetService;
        private readonly IBudgetCalculationService budgetCalculationService;

        public BudgetController(
            IBudgetService budgetService,
            IBudgetCalculationService budgetCalculationService
            )
        {
            this.budgetService = budgetService;
            this.budgetCalculationService = budgetCalculationService;
        }

        [HttpGet("all")]
        public async ValueTask<IActionResult> GetBudgets()
        {
            var userIdentifier = GetUserId();

            if (userIdentifier is null)
                throw new UnauthorizedAccessException();

            var userId = userIdentifier.Value;

            var budgets = await budgetService.RetrieveAllBudgetsAsync(userId);

            return Ok(budgets);
        }

        [HttpGet("get/{budgetId}")]
        public async ValueTask<IActionResult> GetBudgetById([FromRoute] Guid budgetId)
        {
            var userIdentifier = GetUserId();

            if (userIdentifier is null)
                throw new UnauthorizedAccessException();

            var userId = userIdentifier.Value;

            var budget = await budgetService
                .RetrieveBudgetByIdAsync(userId, budgetId);

            return budget is null
                ? NotFound()
                : Ok(budget);
        }

        [HttpPost("add")]
        public async ValueTask<IActionResult> CreateBudget([FromBody] BudgetCreateDto budget)
        {
            var userIdentifier = GetUserId();

            if (userIdentifier is null)
                throw new UnauthorizedAccessException();

            var userId = userIdentifier.Value;

            var createdBudget = await budgetService
                .CreateBudgetAsync(userId, budget);

            return Created(createdBudget);
        }

        [HttpPut("update/{budgetId}")]
        public async ValueTask<IActionResult> UpdateBudget([FromRoute] Guid budgetId, [FromBody] BudgetUpdateDto budget)
        {
            var userIdentifier = GetUserId();

            if (userIdentifier is null)
                throw new UnauthorizedAccessException();

            var userId = userIdentifier.Value;

            var updatedBudget = await budgetService
                .UpdateBudgetAsync(budgetId, userId, budget);

            return updatedBudget is null
                ? NotFound()
                : Ok(updatedBudget);
        }

        [HttpDelete("delete/{budgetId}")]
        public async ValueTask<IActionResult> DeleteBudget([FromRoute]Guid budgetId)
        {
            var userIdentifier = GetUserId();

            if (userIdentifier is null)
                throw new UnauthorizedAccessException();

            var userId = userIdentifier.Value;

            var isDeleted = await budgetService
                .DeleteBudgetAsync(userId, budgetId);

            return isDeleted
                ? NoContent()
                : NotFound();
        }

        [HttpGet("stats/{budgetId}")]
        public async ValueTask<IActionResult> GetBudgetStats([FromRoute] Guid budgetId)
        {
            var userIdentifier = GetUserId();
            if (userIdentifier is null)
                throw new UnauthorizedAccessException();

            var userId = userIdentifier.Value;

            var stats = await budgetCalculationService.GetBudgetStatsAsync(userId, budgetId);
            return Ok(stats);
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
