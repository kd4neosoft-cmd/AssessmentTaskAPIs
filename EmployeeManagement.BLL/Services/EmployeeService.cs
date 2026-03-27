using EmployeeManagement.Common.Interfaces;
using EmployeeManagement.Common.Models;
using EmployeeManagement.DAL.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.BLL.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<EmployeeDto, int> _repository;
        private readonly IFileService _fileService;

        public EmployeeService(IRepository<EmployeeDto, int> repository, IFileService fileService)
        {
            _repository = repository;
            _fileService = fileService;
        }

        public async Task<ApiResponse<PaginatedResult<EmployeeDto>>> GetEmployeesAsync(GridRequest request)
        {
            try
            {
                var (items, totalCount) = await _repository.GetPagedAsync(
                    request.PageNumber, request.PageSize, request.SearchTerm,
                    request.SortBy ?? "Row_Id", request.SortOrder);

                return ApiResponse<PaginatedResult<EmployeeDto>>.SuccessResponse(
                    new PaginatedResult<EmployeeDto> { Items = items, TotalCount = totalCount },
                    pagination: new PaginationInfo
                    {
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalCount = totalCount
                    });
            }
            catch (Exception ex)
            {
                return ApiResponse<PaginatedResult<EmployeeDto>>.ErrorResponse(
                    $"Failed to retrieve employees: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<EmployeeDto?>> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var employee = await _repository.GetByIdAsync(id);
                return employee == null
                    ? ApiResponse<EmployeeDto?>.ErrorResponse("Employee not found", 404)
                    : ApiResponse<EmployeeDto?>.SuccessResponse(employee);
            }
            catch (Exception ex)
            {
                return ApiResponse<EmployeeDto?>.ErrorResponse(
                    $"Failed to retrieve employee: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<int>> AddEmployeeAsync(EmployeeCreateRequest request, IFormFile? profileImage)
        {
            try
            {
                var validationResult = ValidateEmployee(request, profileImage, isNew: true);
                if (!validationResult.Success)
                    return ApiResponse<int>.ErrorResponse(validationResult.Message, 400);

                string? profileImagePath = null;
                if (profileImage != null)
                {
                    var uploadResult = await _fileService.UploadEmployeeImageAsync(profileImage);
                    if (!uploadResult.Success)
                        return ApiResponse<int>.ErrorResponse(uploadResult.Message, 400);
                    profileImagePath = uploadResult.FilePath;
                }

                var employee = new EmployeeDto
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CountryId = request.CountryId,
                    StateId = request.StateId,
                    CityId = request.CityId,
                    EmailAddress = request.EmailAddress.ToLower(),
                    MobileNumber = request.MobileNumber,
                    PanNumber = request.PanNumber.ToUpper(),
                    PassportNumber = request.PassportNumber.ToUpper(),
                    ProfileImage = profileImagePath,
                    Gender = request.Gender,
                    IsActive = request.IsActive,
                    DateOfBirth = request.DateOfBirth,
                    DateOfJoinee = request.DateOfJoinee
                };

                var newRowId = await _repository.AddAsync(employee);
                return ApiResponse<int>.SuccessResponse(newRowId, "Employee created successfully", 201);
            }
            catch (ApplicationException ex)
            {
                return ApiResponse<int>.ErrorResponse(ex.Message, 409);
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Failed to add employee: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> UpdateEmployeeAsync(EmployeeUpdateRequest request, IFormFile? profileImage)
        {
            try
            {
                var validationResult = ValidateEmployee(request, profileImage, isNew: false, request.Row_Id);
                if (!validationResult.Success)
                    return ApiResponse<bool>.ErrorResponse(validationResult.Message, 400);

                var existing = await _repository.GetByIdAsync(request.Row_Id);
                if (existing == null)
                    return ApiResponse<bool>.ErrorResponse("Employee not found", 404);

                string? profileImagePath = existing.ProfileImage;
                if (profileImage != null)
                {
                    if (!string.IsNullOrEmpty(existing.ProfileImage))
                        _fileService.DeleteFile(existing.ProfileImage);

                    var uploadResult = await _fileService.UploadEmployeeImageAsync(profileImage);
                    if (!uploadResult.Success)
                        return ApiResponse<bool>.ErrorResponse(uploadResult.Message, 400);
                    profileImagePath = uploadResult.FilePath;
                }

                var employee = new EmployeeDto
                {
                    Row_Id = request.Row_Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CountryId = request.CountryId,
                    StateId = request.StateId,
                    CityId = request.CityId,
                    EmailAddress = request.EmailAddress.ToLower(),
                    MobileNumber = request.MobileNumber,
                    PanNumber = request.PanNumber.ToUpper(),
                    PassportNumber = request.PassportNumber.ToUpper(),
                    ProfileImage = profileImagePath,
                    Gender = request.Gender,
                    IsActive = request.IsActive,
                    DateOfBirth = request.DateOfBirth,
                    DateOfJoinee = request.DateOfJoinee
                };

                await _repository.UpdateAsync(employee);
                return ApiResponse<bool>.SuccessResponse(true, "Employee updated successfully");
            }
            catch (ApplicationException ex)
            {
                return ApiResponse<bool>.ErrorResponse(ex.Message, 409);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Failed to update employee: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteEmployeeAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return ApiResponse<bool>.SuccessResponse(true, "Employee deleted successfully");
            }
            catch (ApplicationException ex)
            {
                return ApiResponse<bool>.ErrorResponse(ex.Message, 404);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Failed to delete employee: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<LocationDto>>> GetCountriesAsync()
        {
            try
            {
                if (_repository is EmployeeRepository empRepo)
                {
                    var countries = await empRepo.GetCountriesAsync();
                    return ApiResponse<IEnumerable<LocationDto>>.SuccessResponse(countries);
                }
                return ApiResponse<IEnumerable<LocationDto>>.ErrorResponse("Operation not supported", 501);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<LocationDto>>.ErrorResponse(
                    $"Failed to retrieve countries: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<LocationDto>>> GetStatesByCountryAsync(int countryId)
        {
            try
            {
                if (_repository is EmployeeRepository empRepo)
                {
                    var states = await empRepo.GetStatesByCountryAsync(countryId);
                    return ApiResponse<IEnumerable<LocationDto>>.SuccessResponse(states);
                }
                return ApiResponse<IEnumerable<LocationDto>>.ErrorResponse("Operation not supported", 501);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<LocationDto>>.ErrorResponse(
                    $"Failed to retrieve states: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<LocationDto>>> GetCitiesByStateAsync(int stateId)
        {
            try
            {
                if (_repository is EmployeeRepository empRepo)
                {
                    var cities = await empRepo.GetCitiesByStateAsync(stateId);
                    return ApiResponse<IEnumerable<LocationDto>>.SuccessResponse(cities);
                }
                return ApiResponse<IEnumerable<LocationDto>>.ErrorResponse("Operation not supported", 501);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<LocationDto>>.ErrorResponse(
                    $"Failed to retrieve cities: {ex.Message}", 500);
            }
        }

        private (bool Success, string Message) ValidateEmployee(EmployeeCreateRequest request,
            IFormFile? profileImage, bool isNew, int? existingId = null)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || request.FirstName.Length < 2)
                return (false, "First name is required (min 2 characters)");

            if (string.IsNullOrWhiteSpace(request.EmailAddress) || !IsValidEmail(request.EmailAddress))
                return (false, "Valid email address is required");

            if (string.IsNullOrWhiteSpace(request.MobileNumber) || !IsValidMobile(request.MobileNumber))
                return (false, "Valid 10-digit mobile number is required");

            if (string.IsNullOrWhiteSpace(request.PanNumber) || !IsValidPan(request.PanNumber))
                return (false, "Valid PAN (ABCDE1234F) is required");

            if (string.IsNullOrWhiteSpace(request.PassportNumber) || request.PassportNumber.Length < 8)
                return (false, "Valid passport number (min 8 chars) is required");

            if (request.DateOfBirth >= DateTime.Today)
                return (false, "Date of birth must be in the past");

            if (request.DateOfJoinee.HasValue && request.DateOfJoinee.Value >= DateTime.Today)
                return (false, "Date of joining must be in the past");

            if (profileImage != null)
            {
                var allowedTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedTypes.Contains(profileImage.ContentType.ToLower()))
                    return (false, "Only JPG or PNG images are allowed");

                const int maxSize = 200 * 1024; // 200 KB
                if (profileImage.Length > maxSize)
                    return (false, "Image size must not exceed 200 KB");
            }

            return (true, string.Empty);
        }

        private bool IsValidEmail(string email) =>
            !string.IsNullOrWhiteSpace(email) && email.Contains("@") && email.Contains(".");

        private bool IsValidMobile(string mobile) =>
            !string.IsNullOrWhiteSpace(mobile) && System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^[0-9]{10}$");

        private bool IsValidPan(string pan) =>
            !string.IsNullOrWhiteSpace(pan) && System.Text.RegularExpressions.Regex.IsMatch(pan.ToUpper(), @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$");
    }
}
