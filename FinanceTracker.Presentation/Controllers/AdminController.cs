using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using System.Linq.Expressions;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : RESTFulController
    {
        private readonly IAdminService adminService;

        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        [HttpGet("users")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult GetUsers([FromQuery] string search, [FromQuery] bool? isActive)
        {
            string lowerSearch = search.ToLower();
            Expression<Func<User, bool>> filter = user =>
                    (string.IsNullOrEmpty(lowerSearch) || 
                    user.Email.ToLower().Contains(lowerSearch) ||
                    user.FirstName.ToLower().Contains(lowerSearch) ||
                    user.LastName.ToLower().Contains(lowerSearch)) &&
                    (!isActive.HasValue || user.IsActive == isActive);

            var users = adminService.GetAllUsers(filter).Select(user => new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Role,
                user.CreatedAt,
                user.IsActive
            });

            return Ok(users);
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPatch("block-user/{userId}")]
        public async ValueTask<ActionResult> BlockUser(Guid userId)
        {
            await adminService.BlockUserAsync(userId);

            return NoContent();
        }

        [HttpPatch("unblock-user/{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async ValueTask<ActionResult> UnblockUser(Guid userId)
        {
            await adminService.UnBlockUserAsync(userId);
            return NoContent();
        }

        [HttpPatch("update-role/{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async ValueTask<ActionResult> UpdateRole(Guid userId, Role role)
        {
            await adminService.UpdateUserRoleAsync(userId, role);

            return NoContent();
        }

    }
}
