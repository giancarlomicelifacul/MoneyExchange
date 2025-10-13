using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Cambio.Infra
{
    public class DatabaseConfig
    {
        private const string ConnectionString = "Data Source=exchange_history.db";

        public static IDbConnection GetConnection()
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public static void Initialize()
        {
            using var connection = GetConnection();
            var sql = @"
                CREATE TABLE IF NOT EXISTS ExchangeHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FromCurrency TEXT NOT NULL,
                    ToCurrency TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    ConvertedAmount REAL NOT NULL,
                    CreatedAt TEXT NOT NULL
                );
            ";
            connection.Execute(sql);
        }
    }
}
