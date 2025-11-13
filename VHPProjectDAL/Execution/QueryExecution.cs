using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace VHPProjectDAL.Execution
{
    public static class QueryExecution
    {
        public static async Task<T?> ExecuteScalarAsync<T>(
       this DatabaseFacade database,
       string sql,
       Func<DbDataReader, T> mapper,
       params MySqlParameter[] parameters)
        {
            var connection = database.GetDbConnection();
            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.Parameters.AddRange(parameters);

                await using var reader = await command.ExecuteReaderAsync();
                return await reader.ReadAsync() ? mapper(reader) : default;
            }
            finally
            {
                //Only close if EF didn't already manage it.
                if (database.GetDbConnection().State == ConnectionState.Open)
                    await database.CloseConnectionAsync();
            }

            //await using var connection = database.GetDbConnection();
            //await connection.OpenAsync();

            //await using var command = connection.CreateCommand();
            //command.CommandText = sql;
            //command.CommandType = CommandType.Text;
            //foreach (var p in parameters) command.Parameters.Add(p);

            //await using var reader = await command.ExecuteReaderAsync();
            //return await reader.ReadAsync() ? mapper(reader) : default;
        }
    }

}
