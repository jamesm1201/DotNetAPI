using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace DotNetAPI.Data 
{
    class DataContextDapper{
        private readonly IConfiguration _config;

        //Constructor sets private config field to be same as the one passed to it
        public DataContextDapper(IConfiguration config){
            _config = config;
        }

        //Reusable dapper class to load data of any type (T)
        public IEnumerable<T> LoadData<T>(string sql){
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }
        public T LoadDataSingle<T>(string sql){
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSql(string sql){
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql) > 0;
        }

         public int ExecuteSqlWithRowCount(string sql){
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql);
        }

         public bool ExecuteSqlWithParams(string sql, List<SqlParameter> parameters){
            SqlCommand commandWithParams = new SqlCommand(sql);
            foreach(SqlParameter parameter in parameters){
                commandWithParams.Parameters.Add(parameter);
            }
            //Don't use IDbConnection
            SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            dbConnection.Open();
            commandWithParams.Connection = dbConnection;
            int rowsAffected = commandWithParams.ExecuteNonQuery();
            //Remember to open and close connection properly
            dbConnection.Close();
            return rowsAffected > 0;

        }
    }
}