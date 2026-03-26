using EmployeeManagement.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.BLL.Services
{
    public class FileService : IFileService
    {
        private readonly IHostEnvironment _env; 
        private const string UploadFolder = "Uploads/Employee";
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxFileSize = 200 * 1024; // 200 KB

        public FileService(IHostEnvironment env)  // ← Updated constructor
        {
            _env = env;
        }

        public async Task<(bool Success, string Message, string? FilePath)> UploadEmployeeImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "No file provided", null);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return (false, $"Invalid file type. Allowed: {string.Join(", ", AllowedExtensions)}", null);

            if (file.Length > MaxFileSize)
                return (false, $"File size exceeds {MaxFileSize / 1024} KB limit", null);

            // Create unique filename
            var uniqueName = $"{Guid.NewGuid()}_{SanitizeFileName(Path.GetFileNameWithoutExtension(file.FileName))}{extension}";

            var uploadsPath = Path.Combine(_env.ContentRootPath, UploadFolder);
            Directory.CreateDirectory(uploadsPath);

            var filePath = Path.Combine(uploadsPath, uniqueName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var relativePath = $"/uploads/Employee/{uniqueName}";
            return (true, "File uploaded successfully", relativePath);
        }

        public bool DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;

            try
            {
                // Use ContentRootPath for consistency
                var physicalPath = Path.Combine(_env.ContentRootPath, filePath.TrimStart('/'));

                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                //_logger?.LogWarning(ex, "Failed to delete file: {FilePath}", filePath);
            }
            return false;
        }

        private string SanitizeFileName(string fileName)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(c, '_');
            return fileName;
        }
    }
}
