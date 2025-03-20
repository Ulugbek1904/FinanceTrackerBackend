using Microsoft.AspNetCore.Http;

namespace FinanceTracker.Infrastructure.Providers.FileProvider
{
    public interface IFileStorageProvider
    {
        public ValueTask<string> UploadFileAsync(IFormFile file);
        public ValueTask DeleteFileAsync(string filePath);
    }
}
