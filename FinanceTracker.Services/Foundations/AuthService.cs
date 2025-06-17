using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Infrastructure.Providers.AuthProvider;
using FinanceTracker.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FinanceTracker.Services.Foundations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthProvider provider;
        private readonly IUserService userService;
        private readonly IHttpContextAccessor contextAccessor;
        public AuthService(
            IAuthProvider provider,
            IUserService userService,
            IHttpContextAccessor contextAccessor)
        {
            this.provider = provider;
            this.userService = userService;
            this.contextAccessor = contextAccessor;
        }

        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            var user = await this.userService.GetUserByEmailAsync(email);

            if(user is null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials");

            bool isValidPassword = BCrypt.Net.
                BCrypt.Verify(password, user.HashedPassword);

            if (!isValidPassword)
                throw new UnauthorizedAccessException("Invalid credentials");


            var accessToken = this.provider.GenerateJwtToken(user);
            var refreshToken = this.provider.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            await this.userService.ModifyUserAsync(user);

            return  new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiryDate = DateTime.UtcNow.AddMinutes(15)
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var user = await userService.RetrieveUserByRefreshTokenAsync(refreshToken)
                ?? throw new SecurityTokenException("Invalid or expired refresh token.");

            if (user.RefreshTokenExpiration <= DateTime.UtcNow)
                throw new SecurityTokenException("Refresh token expired. Please log in again.");

            var newAccessToken = provider.GenerateJwtToken(user);
            var newRefreshToken = provider.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await userService.ModifyUserAsync(user);

            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiryDate = DateTime.UtcNow.AddMinutes(30)
            };
        }

        public async ValueTask RevokeAsync()
        {
            var userClaims = this.contextAccessor.
                HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

            if(userClaims == null)
                throw new UnauthorizedAccessException(" User not found");

            var userId = Guid.Parse(userClaims.Value);
            var user = await this.userService.
                RetrieveUserByIdAsync(userId);

            user.RefreshToken = null;
            user.RefreshTokenExpiration = null;

            await this.userService.ModifyUserAsync(user);
        }

    }

    
}