﻿using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Exceptions;
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
                user.HashedPassword,
                user.LastName,
                user.Role,
                user.CreatedAt,
                user.IsActive
            });

            return Ok(users);
        }

        [HttpPost("create-user")]
        [Authorize(Roles = "SuperAdmin")]
        public async ValueTask<IActionResult> RegisterUser(CreateUserDto userDto)
        {
            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var admin = await userService.RetrieveUserByIdAsync(adminId);
            if (admin == null)
                throw new ForbiddenAccessException("you are not admin");

            var existedUser = await userService.GetUserByEmailAsync(userDto.Email);
            if (existedUser is not null)
            {
                return BadRequest(new
                {
                    message = "A user with this email already exists",
                    result = false,
                    data = (object)null
                });
            }

            var user = new User
            {
                Email = userDto.Email,
                HashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Role = Role.User,
                CreatedBy = admin.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdUser = await orchestration.RegisterUserAsync(user);

            return Ok(new
            {
                message = "User successfully created",
                result = true,
                data = createdUser
            });
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
