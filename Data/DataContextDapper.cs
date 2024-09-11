using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace DotNetAPI{
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
    }
}