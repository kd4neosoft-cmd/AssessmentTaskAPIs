using EmployeeManagement.Common.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Common.Interfaces
{
    public interface IEmployeeService
    {
        Task<ApiResponse<PaginatedResult<EmployeeDto>>> GetEmployeesAsync(GridRequest request);
        Task<ApiResponse<EmployeeDto?>> GetEmployeeByIdAsync(int id);
        Task<ApiResponse<int>> AddEmployeeAsync(EmployeeCreateRequest request, IFormFile? profileImage);
        Task<ApiResponse<bool>> UpdateEmployeeAsync(EmployeeUpdateRequest request, IFormFile? profileImage);
        Task<ApiResponse<bool>> DeleteEmployeeAsync(int id);
        Task<ApiResponse<IEnumerable<LocationDto>>> GetCountriesAsync();
        Task<ApiResponse<IEnumerable<LocationDto>>> GetStatesByCountryAsync(int countryId);
        Task<ApiResponse<IEnumerable<LocationDto>>> GetCitiesByStateAsync(int stateId);
    }
}
