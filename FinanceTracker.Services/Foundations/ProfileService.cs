using FinanceTracker.Domain.Models;
using FinanceTracker.Infrastructure.Brokers.Storages;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FinanceTracker.Services.Foundations.Interfaces;
using FinanceTracker.Infrastructure.Providers.FileProvider;
using Microsoft.AspNetCore.Identity;
using FinanceTracker.Domain.Models.DTOs.AuthDtos;
using FinanceTracker.Domain.Exceptions;

namespace FinanceTracker.Services.Foundations
{
    public class ProfileService : IProfileService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IFileStorageProvider fileStorage;

        private readonly string ImageStoragePath = "/app/LocalFileStorage";
        private readonly string ImageRequestPath = "/profile-pictures";

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
            if (file == null || file.Length == 0)
                throw new AppException("Fayl topilmadi.");

            const long maxSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxSize)
                throw new AppException("Fayl hajmi juda katta.");

            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                throw new AppException("Faqat .png, .jpg, .jpeg ruxsat etiladi.");

            var userIdClaim = contextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Foydalanuvchi aniqlanmadi.");

            var userId = Guid.Parse(userIdClaim.Value);
            var user = await storageBroker.SelectByIdAsync<User>(userId);

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                var fileNameToDelete = Path.GetFileName(user.ProfilePictureUrl);
                var filePathToDelete = Path.Combine(ImageStoragePath, fileNameToDelete);

                if (File.Exists(filePathToDelete))
                    File.Delete(filePathToDelete);
            }

            var newFileName = $"{Guid.NewGuid()}{fileExtension}";
            var savePath = Path.Combine(ImageStoragePath, newFileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/{ImageRequestPath}/{newFileName}";
            user.ProfilePictureUrl = imageUrl;

            await storageBroker.UpdateAsync(user);

            return $"https://finance-tracker-latest-coiu.onrender.com{imageUrl}";
        }

        public bool Verify(string old, string _new)
        {
            if (old == _new) return true;

            return false;
        }
    }
}
