using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace TestingSystem.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Выполнение запроса с возвратом данных
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            return await Dapper.SqlMapper.QueryAsync<T>(connection, sql, parameters);
        }

        // Выполнение запроса с возвратом одного объекта
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            return await Dapper.SqlMapper.QueryFirstOrDefaultAsync<T>(connection, sql, parameters);
        }

        // Выполнение команды (INSERT, UPDATE, DELETE)
        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            return await Dapper.SqlMapper.ExecuteAsync(connection, sql, parameters);
        }

        // Выполнение команды с возвратом ID вставленной записи
        public async Task<int> ExecuteScalarAsync(string sql, object parameters = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            return await Dapper.SqlMapper.ExecuteScalarAsync<int>(connection, sql, parameters);
        }
        public async Task<T> ExecuteScalarAsync<T>(string sql, object parameters = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            return await Dapper.SqlMapper.ExecuteScalarAsync<T>(connection, sql, parameters);
        }
    }
}