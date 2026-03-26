using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public PaginationInfo? Pagination { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful", int statusCode = 200, PaginationInfo? pagination = null)
            => new() { Success = true, Message = message, StatusCode = statusCode, Data = data, Pagination = pagination };

        public static ApiResponse<T> ErrorResponse(string message, int statusCode = 400)
            => new() { Success = false, Message = message, StatusCode = statusCode };
    }
}
