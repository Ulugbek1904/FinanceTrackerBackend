using FinanceTracker.Domain.Models.DTOs.AuthDtos;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using System.Security.Claims;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController  : RESTFulController
    {
        private readonly IUserService userService;
        private readonly IProfileService profileService;

        public ProfileController(
            IUserService userService,
            IProfileService profileService)
        {
            this.userService = userService;
            this.profileService = profileService;
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDto passwordDto)
        {
            var user = await profileService.ChangePasswordAsync(passwordDto);

            user.UpdatedAt = DateTime.UtcNow;

            await this.userService.ModifyUserAsync(user);

            return Ok();
        }

        [Authorize]
        [HttpPut("update-profile")]
        public  async ValueTask<IActionResult> UpdateProfile(
            string firstName, 
            string lastName)
        {
            var userId = Guid.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User id is missing"));

            var user = await profileService.UpdateProfileAsync(userId, firstName, lastName);
            user.UpdatedAt = DateTime.UtcNow;
            await this.userService.ModifyUserAsync(user);
            return Ok();
        }

        [Authorize]
        [HttpPost("upload-profile-picture")]
        public async ValueTask<IActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                var pictureUrl = await profileService.UploadProfilePictureAsync(file);
                return Ok(new { pictureUrl = pictureUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UploadAvatar Error: {ex.Message} - {ex.StackTrace}");
                return StatusCode(500, new { error = "Internal server error during upload. Please try again later." });
            }
        }
    }
}
