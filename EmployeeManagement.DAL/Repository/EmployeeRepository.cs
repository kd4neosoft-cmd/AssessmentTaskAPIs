using EmployeeManagement.Common.Interfaces;
using EmployeeManagement.Common.Models;
using EmployeeManagement.DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.DAL.Repository
{
    public class EmployeeRepository : IRepository<EmployeeDto, int>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EmployeeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var parameters = new[] { new SqlParameter("@Row_Id", id) };

            var employees = await SqlHelper.QueryAsync(_connectionFactory,
                "stp_Emp_GetById", MapEmployee, parameters);

            return employees.FirstOrDefault();
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            return await SqlHelper.QueryAsync(_connectionFactory,
                "stp_Emp_GetAll", MapEmployee,
                new SqlParameter("@PageNumber", 1),
                new SqlParameter("@PageSize", int.MaxValue));
        }

        public async Task<(IEnumerable<EmployeeDto> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, string? searchTerm, string sortBy, string sortOrder)
        {
            var parameters = new[]
            {
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@SearchTerm", (object?)searchTerm ?? DBNull.Value),
                new SqlParameter("@SortBy", sortBy ?? "Row_Id"),
                new SqlParameter("@SortOrder", sortOrder ?? "ASC")
            };

            return await SqlHelper.QueryPagedAsync(_connectionFactory,
                "stp_Emp_GetAll", MapEmployee, parameters);
        }

        public async Task<int> AddAsync(EmployeeDto entity)
        {
            var employeeCodeParam = new SqlParameter("@EmployeeCode", SqlDbType.VarChar, 8)
            { Direction = ParameterDirection.Output };
            var messageParam = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 500)
            { Direction = ParameterDirection.Output };
            var codeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
            { Direction = ParameterDirection.Output };

            var parameters = new[]
            {
                employeeCodeParam,
                new SqlParameter("@FirstName", entity.FirstName),
                new SqlParameter("@LastName", (object?)entity.LastName ?? DBNull.Value),
                new SqlParameter("@CountryId", entity.CountryId),
                new SqlParameter("@StateId", entity.StateId),
                new SqlParameter("@CityId", entity.CityId),
                new SqlParameter("@EmailAddress", entity.EmailAddress),
                new SqlParameter("@MobileNumber", entity.MobileNumber),
                new SqlParameter("@PanNumber", entity.PanNumber.ToUpper()),
                new SqlParameter("@PassportNumber", entity.PassportNumber.ToUpper()),
                new SqlParameter("@ProfileImage", (object?)entity.ProfileImage ?? DBNull.Value),
                new SqlParameter("@Gender", entity.Gender),
                new SqlParameter("@IsActive", entity.IsActive),
                new SqlParameter("@DateOfBirth", entity.DateOfBirth),
                new SqlParameter("@DateOfJoinee", (object?)entity.DateOfJoinee ?? DBNull.Value),
                messageParam,
                codeParam
            };

            var newRowId = await SqlHelper.ExecuteScalarAsync<int?>(_connectionFactory,
                "stp_Emp_Insert", parameters);

            var resultCode = (int)(codeParam.Value ?? 500);
            if (resultCode != 201)
                throw new ApplicationException(messageParam.Value?.ToString() ?? "Insert failed");

            return newRowId ?? 0;
        }

        public async Task<bool> UpdateAsync(EmployeeDto entity)
        {
            var messageParam = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 500)
            { Direction = ParameterDirection.Output };
            var codeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
            { Direction = ParameterDirection.Output };

            var parameters = new[]
            {
                new SqlParameter("@Row_Id", entity.Row_Id),
                new SqlParameter("@FirstName", entity.FirstName),
                new SqlParameter("@LastName", (object?)entity.LastName ?? DBNull.Value),
                new SqlParameter("@CountryId", entity.CountryId),
                new SqlParameter("@StateId", entity.StateId),
                new SqlParameter("@CityId", entity.CityId),
                new SqlParameter("@EmailAddress", entity.EmailAddress),
                new SqlParameter("@MobileNumber", entity.MobileNumber),
                new SqlParameter("@PanNumber", entity.PanNumber.ToUpper()),
                new SqlParameter("@PassportNumber", entity.PassportNumber.ToUpper()),
                new SqlParameter("@ProfileImage", (object?)entity.ProfileImage ?? DBNull.Value),
                new SqlParameter("@Gender", entity.Gender),
                new SqlParameter("@IsActive", entity.IsActive),
                new SqlParameter("@DateOfBirth", entity.DateOfBirth),
                new SqlParameter("@DateOfJoinee", (object?)entity.DateOfJoinee ?? DBNull.Value),
                messageParam,
                codeParam
            };

            await SqlHelper.ExecuteNonQueryAsync(_connectionFactory, "stp_Emp_Update", parameters);

            var resultCode = (int)(codeParam.Value ?? 500);
            if (resultCode != 200)
                throw new ApplicationException(messageParam.Value?.ToString() ?? "Update failed");

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var messageParam = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 500)
            { Direction = ParameterDirection.Output };
            var codeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
            { Direction = ParameterDirection.Output };

            var parameters = new[]
            {
                new SqlParameter("@Row_Id", id),
                messageParam,
                codeParam
            };

            await SqlHelper.ExecuteNonQueryAsync(_connectionFactory, "stp_Emp_Delete", parameters);

            var resultCode = (int)(codeParam.Value ?? 500);
            if (resultCode != 200)
                throw new ApplicationException(messageParam.Value?.ToString() ?? "Delete failed");

            return true;
        }

        public async Task<IEnumerable<LocationDto>> GetCountriesAsync()
        {
            return await SqlHelper.QueryAsync(_connectionFactory,
                "stp_Emp_GetCountries", MapLocation);
        }

        public async Task<IEnumerable<LocationDto>> GetStatesByCountryAsync(int countryId)
        {
            var parameters = new[] { new SqlParameter("@CountryId", countryId) };
            return await SqlHelper.QueryAsync(_connectionFactory,
                "stp_Emp_GetStatesByCountry", MapLocation, parameters);
        }

        public async Task<IEnumerable<LocationDto>> GetCitiesByStateAsync(int stateId)
        {
            var parameters = new[] { new SqlParameter("@StateId", stateId) };
            return await SqlHelper.QueryAsync(_connectionFactory,
                "stp_Emp_GetCitiesByState", MapLocation, parameters);
        }

        private static EmployeeDto MapEmployee(IDataReader reader)
        {
            return new EmployeeDto
            {
                Row_Id = reader.GetInt32(reader.GetOrdinal("Row_Id")),
                EmployeeCode = reader.GetString(reader.GetOrdinal("EmployeeCode")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                CountryId = reader.GetInt32(reader.GetOrdinal("CountryId")),
                StateId = reader.GetInt32(reader.GetOrdinal("StateId")),
                CityId = reader.GetInt32(reader.GetOrdinal("CityId")),
                EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                MobileNumber = reader.GetString(reader.GetOrdinal("MobileNumber")),
                PanNumber = reader.GetString(reader.GetOrdinal("PanNumber")),
                PassportNumber = reader.GetString(reader.GetOrdinal("PassportNumber")),
                ProfileImage = reader.IsDBNull(reader.GetOrdinal("ProfileImage")) ? null : reader.GetString(reader.GetOrdinal("ProfileImage")),
                Gender = reader.GetByte(reader.GetOrdinal("Gender")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                DateOfJoinee = reader.IsDBNull(reader.GetOrdinal("DateOfJoinee")) ? null : reader.GetDateTime(reader.GetOrdinal("DateOfJoinee")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
            };
        }

        private static LocationDto MapLocation(IDataReader reader)
        {
            return new LocationDto
            {
                Row_Id = reader.GetInt32(reader.GetOrdinal("Row_Id")),
                Name = reader.GetString(reader.GetOrdinal(reader.GetName(1))) 
            };
        }
    }
}
