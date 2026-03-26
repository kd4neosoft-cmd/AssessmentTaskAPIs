using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Common.Interfaces
{
    public interface IFileService
    {
        Task<(bool Success, string Message, string? FilePath)> UploadEmployeeImageAsync(IFormFile file);
        bool DeleteFile(string filePath);
    }
}
