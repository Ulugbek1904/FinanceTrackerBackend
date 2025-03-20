﻿using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace FinanceTracker.Services.Foundations.Interfaces
{
    public interface IProfileService
    {
        bool Verify(string old, string _new);
        Task<User> ChangePasswordAsync(ChangePasswordDto passwordDto);
        ValueTask<User> UpdateProfileAsync(Guid userId, string firstName, string lastName);
        ValueTask<string> UploadProfilePictureAsync(IFormFile file);
    }
}
