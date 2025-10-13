using Cambio.Service;
using Dapper;
using System.Text;

namespace Cambio.Infra.Repository
{
    public class ExchangeHistoryRepository
    {
        public void Save(ExchangeResultDto exchange)
        {
            using var connection = DatabaseConfig.GetConnection();
            var sql = @"
                INSERT INTO ExchangeHistory 
                (FromCurrency, ToCurrency, Amount, ConvertedAmount, CreatedAt)
                VALUES (@FromCurrency, @ToCurrency, @Amount, @ConvertedAmount, @CreatedAt);
            ";

            connection.Execute(sql, new
            {
                exchange.FromCurrency,
                exchange.ToCurrency,
                exchange.Amount,
                exchange.ConvertedAmount,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        public IEnumerable<dynamic> GetHistory(
            string? fromCurrency = null,
            string? toCurrency = null,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            decimal? minConverted = null,
            decimal? maxConverted = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            using var connection = DatabaseConfig.GetConnection();

            var sql = new StringBuilder("SELECT * FROM ExchangeHistory WHERE 1=1 ");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(fromCurrency))
            {
                sql.Append("AND FromCurrency = @FromCurrency ");
                parameters.Add("@FromCurrency", fromCurrency.ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(toCurrency))
            {
                sql.Append("AND ToCurrency = @ToCurrency ");
                parameters.Add("@ToCurrency", toCurrency.ToUpper());
            }

            if (minAmount.HasValue)
            {
                sql.Append("AND Amount >= @MinAmount ");
                parameters.Add("@MinAmount", minAmount.Value);
            }

            if (maxAmount.HasValue)
            {
                sql.Append("AND Amount <= @MaxAmount ");
                parameters.Add("@MaxAmount", maxAmount.Value);
            }

            if (minConverted.HasValue)
            {
                sql.Append("AND ConvertedAmount >= @MinConverted ");
                parameters.Add("@MinConverted", minConverted.Value);
            }

            if (maxConverted.HasValue)
            {
                sql.Append("AND ConvertedAmount <= @MaxConverted ");
                parameters.Add("@MaxConverted", maxConverted.Value);
            }

            if (startDate.HasValue)
            {
                sql.Append("AND datetime(CreatedAt) >= datetime(@StartDate) ");
                parameters.Add("@StartDate", startDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            if (endDate.HasValue)
            {
                sql.Append("AND datetime(CreatedAt) <= datetime(@EndDate) ");
                parameters.Add("@EndDate", endDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            sql.Append("ORDER BY datetime(CreatedAt) DESC;");

            return connection.Query(sql.ToString(), parameters);
        }
    }
}
