using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using RESTFulSense.Controllers;
using System.Linq.Expressions;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : RESTFulController
    {
        private readonly IAdminService adminService;
        private readonly IUserOrchestration orchestration;
        private readonly IUserService userService;

        public AdminController(
            IAdminService adminService,
            IUserOrchestration orchestration,
            IUserService userService)
        {
            this.adminService = adminService;
            this.orchestration = orchestration;
            this.userService = userService;
        }

        [HttpGet("users")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult GetUsers([FromQuery] string? search, [FromQuery] bool? isActive)
        {

            string lowerSearch = search?.ToLower()??"";
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

        [HttpPost("create-user")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async ValueTask<IActionResult> RegisterUser(CreateUserDto userDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(adminId))
                return Unauthorized();

            var admin = await userService
                .RetrieveUserByIdAsync(Guid.Parse(adminId));

            if (admin == null)
                return Unauthorized();

            var existedUser = await
                this.userService.GetUserByEmailAsync(userDto.Email);

            if (existedUser is not null)
                return BadRequest("A user with this email already exists");
            
            var user = new User
            {
                Email = userDto.Email,
                Password = userDto.Password,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Role = Role.User,
                CreatedBy = admin.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdUser = await this.
                orchestration.RegisterUserAsync(user);

            return Ok(createdUser);
        }

        [HttpDelete("delete-user/{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async ValueTask<ActionResult> DeleteUser(Guid userId)
        {
            await userService.RemoveUserByIdAsync(userId);

            return NoContent();
        }

        [HttpPatch("block-user/{userId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
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
