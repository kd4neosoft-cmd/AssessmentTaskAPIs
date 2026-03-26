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
    public sealed class DbConnectionFactory : IDbConnectionFactory
    {
        private static readonly Lazy<DbConnectionFactory> _instance =
            new(() => new DbConnectionFactory());

        private readonly string _connectionString;
        private static readonly object _lock = new();

        public static DbConnectionFactory Instance => _instance.Value;

        private DbConnectionFactory()
        {
            // Load from configuration (passed via constructor in real app)
            _connectionString = "Server=localhost;Database=EmployeeDB;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        // For DI - allows connection string injection
        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }
    }
}
