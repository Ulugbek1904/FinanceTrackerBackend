using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTFulSense.Controllers;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/category")]
    [Authorize]
    public class CategoryController : RESTFulController
    {
        private readonly ICategoryOrchestrationService categoryOrchestration;
        private readonly ICategoryService categoryService;

        public CategoryController(
            ICategoryOrchestrationService categoryOrchestration,
            ICategoryService categoryService)
        {
            this.categoryOrchestration = categoryOrchestration;
            this.categoryService = categoryService;
        }

        [HttpGet("all")]
        public async ValueTask<IActionResult> GetCategoriesAsync()
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var categories = await categoryOrchestration.
                    RetrieveCategoriesByUserId(userId.Value).ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async ValueTask<IActionResult> GetCategoryByIdAsync([FromRoute] int id)
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var category = await categoryOrchestration.
                    RetrieveCategoriesByUserId(userId.Value).FirstOrDefaultAsync(c => c.Id == id);

                if (category is null)
                    return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("create-category")]
        public async ValueTask<IActionResult> PostCategoryAsync([FromBody] CategoryDto categoryDto)
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var categories = await categoryOrchestration
                    .RetrieveCategoriesByUserId(userId.Value)
                    .ToListAsync();

                var existingCategory = categories
                    .FirstOrDefault(c => c.Name == categoryDto.Name);

                if (existingCategory != null)
                {
                    return BadRequest("Category already exists");
                }

                var category = new Category
                {
                    Name = categoryDto.Name,
                    IsIncome = categoryDto.IsIncome,
                    UserId = userId,
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var created = await this.categoryOrchestration.
                    AddCategoryAsync(userId.Value, category);

                return Created(created);

            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("update")]
        public async ValueTask<IActionResult> UpdateCategoryAsync(int id,[FromBody] CategoryDto categoryDto)
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();

                var existingCategory = await categoryService.RetrieveCategoryByIdAsync(id);

                if (existingCategory is null || existingCategory.UserId != userId)
                    return NotFound();

                existingCategory.Name = categoryDto.Name;
                existingCategory.IsIncome = categoryDto.IsIncome;
                existingCategory.UpdatedAt = DateTime.UtcNow;

                var updated = await categoryOrchestration.UpdateCategoryAsync(userId.Value, existingCategory);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException uex)
            {
                return Forbid(uex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async ValueTask<IActionResult> DeleteCategoryAsync([FromRoute] int id)
        {
            try
            {
                var userId = GetUserId();

                if (userId is null)
                    return Unauthorized();
                var category = await categoryService.RetrieveCategoryByIdAsync(id);

                if(category.IsDefault)
                    return BadRequest("Cannot delete default category");

                var deleted = await categoryOrchestration.RemoveCategoryByIdAsync(userId.Value, id);

                return Ok(deleted);
            }
            catch (UnauthorizedAccessException uex)
            {
                return Forbid(uex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private Guid? GetUserId()
        {
            var userId = User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return string.IsNullOrEmpty
                (userId) ? null : Guid.Parse(userId);
        }
    }
}
