using Microsoft.AspNetCore.Http;

namespace FinanceTracker.Infrastructure.Providers.FileProvider
{
    public class LocalFileStorageProvider : IFileStorageProvider
    {
        private readonly string storagePath = Path.Combine(Directory.GetCurrentDirectory(), "LocalFileStorage");
        public LocalFileStorageProvider()
        {
            Directory.CreateDirectory(storagePath);
        }

        public ValueTask DeleteFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return ValueTask.CompletedTask;
        }

        public async ValueTask<string> UploadFileAsync(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}" +
                $"{Path.GetExtension(file.FileName)}";

            var filePath = Path.Combine(storagePath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return filePath;
        }
    }
}
