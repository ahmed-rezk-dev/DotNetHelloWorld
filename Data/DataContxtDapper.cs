using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace HelloWorld.Data
{
    public class DataContxtDapper
    {
        private string dbConnectionString =
            "Server=localhost;Database=DotNetCourseDatabase;TrustServerCertificate=True;Trusted_Connection=false;User Id=sa;Password=SQLConnect1;";

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(dbConnectionString);
            return dbConnection.Query<T>(sql);
        }

        public T loadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(dbConnectionString);
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool exucuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(dbConnectionString);
            return (dbConnection.Execute(sql) > 0);
        }

        public int exucuteSqlWithRowsCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(dbConnectionString);
            return dbConnection.Execute(sql);
        }
    }
}
