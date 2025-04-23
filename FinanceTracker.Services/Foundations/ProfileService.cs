using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Brokers.Storages;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Infrastructure.Providers.FileProvider;
using Microsoft.AspNetCore.Identity;
using FinanceTracker.Domain.Models.DTOs.AuthDtos;

namespace FinanceTracker.Services.Foundations
{
    public class ProfileService : IProfileService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IFileStorageProvider fileStorage;

        public ProfileService(
            IStorageBroker storageBroker,
            IHttpContextAccessor contextAccessor,
            IFileStorageProvider fileStorage)
        {
            this.storageBroker = storageBroker;
            this.contextAccessor = contextAccessor;
            this.fileStorage = fileStorage;
        }

        public async Task<User> ChangePasswordAsync(ChangePasswordDto passwordDto)
        {
            var userIdClaim = contextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User not found");

            Guid userId = Guid.Parse(userIdClaim.Value);

            var user = await storageBroker.SelectByIdAsync<User>(userId);

            var isVerified = BCrypt.Net.BCrypt
                .Verify(passwordDto.OldPassword, user.HashedPassword);

            if (!isVerified)
            {
                throw new UnauthorizedAccessException("Password is incorrect");
            }

            user.HashedPassword = BCrypt.Net
                .BCrypt.HashPassword(passwordDto.NewPassword);

            await storageBroker.UpdateAsync(user);

            return user;
        }


        public async ValueTask<User> UpdateProfileAsync(Guid userId, string firstName, string lastName)
        {
            var user = await this.storageBroker.SelectByIdAsync<User>(userId);

            user.FirstName = firstName;
            user.LastName = lastName;

            return await this.storageBroker.UpdateAsync(user);
        }

        public async ValueTask<string> UploadProfilePictureAsync(IFormFile file)
        {
            if(file == null || file.Length == 0)
                throw new ArgumentNullException("File is missing");

            const long maxSize = 5 * 1024 * 1024; // 2 MB
            if (file.Length > maxSize)
                throw new ArgumentOutOfRangeException("File size is too large");

            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException($"Type of file is not allowed" +
                    $". Allowed types : {string.Join(", ",allowedExtensions)}");
            }

            var userIdClaim = contextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier);

            if(userIdClaim == null)
                throw new UnauthorizedAccessException("User not found");

            var userId = Guid.Parse(userIdClaim.Value);
            var user = await storageBroker.SelectByIdAsync<User>(userId);

            if(!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                await fileStorage.DeleteFileAsync(user.ProfilePictureUrl);
            }

            user.ProfilePictureUrl = await fileStorage.UploadFileAsync(file);
            await storageBroker.UpdateAsync(user);

            return user.ProfilePictureUrl;
        }

        public bool Verify(string old, string _new)
        {
            if (old == _new) return true;

            return false;
        }
    }
}
