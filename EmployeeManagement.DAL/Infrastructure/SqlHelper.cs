using EmployeeManagement.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.DAL.Infrastructure
{
    public static class SqlHelper
    {
        public static async Task<T?> ExecuteScalarAsync<T>(IDbConnectionFactory connectionFactory,
            string spName, params SqlParameter[] parameters)
        {
            using var connection = connectionFactory.CreateConnection();
            using var command = new SqlCommand(spName, (SqlConnection)connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 30
            };

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            var result = await command.ExecuteScalarAsync();
            return result == null || result == DBNull.Value ? default : (T)Convert.ChangeType(result, typeof(T));
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(IDbConnectionFactory connectionFactory,
            string spName, Func<IDataReader, T> mapper, params SqlParameter[] parameters) where T : class
        {
            var results = new List<T>();

            using var connection = connectionFactory.CreateConnection();
            using var command = new SqlCommand(spName, (SqlConnection)connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60
            };

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(mapper(reader));
            }

            return results;
        }

        public static async Task<(IEnumerable<T> Items, int TotalCount)> QueryPagedAsync<T>(
            IDbConnectionFactory connectionFactory, string spName,
            Func<IDataReader, T> mapper, params SqlParameter[] parameters) where T : class
        {
            var items = new List<T>();
            int totalCount = 0;

            using var connection = connectionFactory.CreateConnection();
            using var command = new SqlCommand(spName, (SqlConnection)connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60
            };

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            // First result: TotalCount
            if (await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(0);
            }

            // Move to next result set: Actual data
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    items.Add(mapper(reader));
                }
            }

            return (items, totalCount);
        }

        public static async Task<int> ExecuteNonQueryAsync(IDbConnectionFactory connectionFactory,
            string spName, params SqlParameter[] parameters)
        {
            using var connection = connectionFactory.CreateConnection();
            using var command = new SqlCommand(spName, (SqlConnection)connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 30
            };

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            return await command.ExecuteNonQueryAsync();
        }
    }
}
