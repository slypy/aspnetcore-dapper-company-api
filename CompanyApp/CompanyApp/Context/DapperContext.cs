using Microsoft.Data.SqlClient;
using System.Data;

namespace CompanyApp.Context
{
    internal class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        internal DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
