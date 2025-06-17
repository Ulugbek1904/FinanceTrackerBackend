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
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException();

            var categories = await categoryOrchestration.
                RetrieveCategoriesByUserId(userId.Value).ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async ValueTask<IActionResult> GetCategoryByIdAsync([FromRoute] int id)
        {
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException();

            var category = await categoryOrchestration.
                RetrieveCategoriesByUserId(userId.Value).FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
                throw new CategoryNotFoundException("Category not found");

            return Ok(category);
        }

        [HttpPost("create-category")]
        public async ValueTask<IActionResult> PostCategoryAsync([FromBody] CategoryDto categoryDto)
        {
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException();

            var categories = await categoryOrchestration
                .RetrieveCategoriesByUserId(userId.Value)
                .ToListAsync();

            var existingCategory = categories
                .FirstOrDefault(c => c.Name == categoryDto.Name);

            if (existingCategory != null)
            {
                throw new AppException("Category already exists");
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

        [HttpPut("update/{id}")]
        public async ValueTask<IActionResult> UpdateCategoryAsync([FromRoute]int id,[FromBody] CategoryDto categoryDto)
        {
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException();

            var existingCategory = await categoryService.RetrieveCategoryByIdAsync(id);

            if (existingCategory is null)
                throw new CategoryNotFoundException($"Not found category with ID : {existingCategory.Id}");

            if (existingCategory.UserId != userId)
                throw new UnauthorizedAccessException();

            existingCategory.Name = categoryDto.Name;
            existingCategory.IsIncome = categoryDto.IsIncome;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            var updated = await categoryOrchestration.UpdateCategoryAsync(userId.Value, existingCategory);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async ValueTask<IActionResult> DeleteCategoryAsync([FromRoute] int id)
        {
            var userId = GetUserId();

            if (userId is null)
                throw new UnauthorizedAccessException();
            var category = await categoryService.RetrieveCategoryByIdAsync(id);

            if (category.IsDefault)
                throw new UnauthorizedAccessException("Cannot delete default category");

            var deleted = await categoryOrchestration.RemoveCategoryByIdAsync(userId.Value, id);

            return Ok(deleted);
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
