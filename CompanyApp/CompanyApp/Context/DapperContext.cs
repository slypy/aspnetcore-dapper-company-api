using Microsoft.Data.SqlClient;
using System.Data;

namespace CompanyApp.Context
{
    /// <summary>
    /// A context wrapper class for our database.
    /// </summary>
    public class DapperContext
    {
        /// <summary>
        /// This service reads key-value pairs from `appsettings`.
        /// </summary>
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        // Inject IConfiguration service
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;

            // Gets the database connection string we have set in appsettings.
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }

        /// <summary>
        /// Creates a database connection using the connection string we have provided in appsettings.
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
