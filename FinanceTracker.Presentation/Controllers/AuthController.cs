﻿using FinanceTracker.Domain.Models.DTOs.AuthDtos;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace FinanceTracker.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : RESTFulController
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            try
            {
                var response = await this.authService.LoginAsync(request.Email, request.Password);

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized("Invalid email or password");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var response = await this.authService.
                RefreshTokenAsync(refreshToken);

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async ValueTask<IActionResult> Logout()
        {
            await this.authService.RevokeAsync();
            return NoContent();
        }
    }
}
