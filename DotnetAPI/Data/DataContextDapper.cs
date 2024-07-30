using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotNetAPI.Data
{
    class DataContextDapper
    {
        private readonly IConfiguration _config;

        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection")
            );
            return dbConnection.Query<T>(sql);
        }

        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection")
            );
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool Execute(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection")
            );
            return dbConnection.Execute(sql) > 0;
        }

        public int ExecuteWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection")
            );
            return dbConnection.Execute(sql);
        }

        public bool ExecuteWithParametersList(string sql, List<SqlParameter> parameters)
        {
            SqlCommand commandWithParameters = new SqlCommand(sql);

            foreach (SqlParameter parameter in parameters)
            {
                commandWithParameters.Parameters.Add(parameter);
            }

            SqlConnection dbConnection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection")
            );
            dbConnection.Open();

            commandWithParameters.Connection = dbConnection;

            int rowsAffected = commandWithParameters.ExecuteNonQuery();

            dbConnection.Close();

            return rowsAffected > 0;
        }

        public bool ExecuteWithParameters<T>(string sql, T parameters)
        {
            Console.WriteLine(parameters);
            IDbConnection dbConnection = new SqlConnection(
                _config.GetConnectionString("DefaultConnection")
            );
            return dbConnection.Execute(sql, parameters) > 0;
        }
    }
}
