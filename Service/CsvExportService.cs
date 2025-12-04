using Cambio.Infra.Repository;
using System.Text;

namespace Cambio.Service
{
    public class CsvExportService
    {
        private readonly ExchangeHistoryRepository _repository;

        public CsvExportService(ExchangeHistoryRepository repository)
        {
            _repository = repository;
        }

        public byte[] GenerateHistoryCsv(
            string? fromCurrency,
            string? toCurrency,
            decimal? minAmount,
            decimal? maxAmount,
            decimal? minConverted,
            decimal? maxConverted,
            DateTime? startDate,
            DateTime? endDate)
        {
            var history = _repository.GetHistory(
                fromCurrency, toCurrency, minAmount, maxAmount,
                minConverted, maxConverted, startDate, endDate
            );

            var csv = new StringBuilder();
            csv.AppendLine("Id,From Currency,To Currency,Amount,Converted Amount,Created At");

            foreach (var item in history)
            {
                var id = item.Id?.ToString() ?? "";
                var from = EscapeCsvField(item.FromCurrency?.ToString() ?? "");
                var to = EscapeCsvField(item.ToCurrency?.ToString() ?? "");
                var amount = item.Amount?.ToString() ?? "";
                var converted = item.ConvertedAmount?.ToString() ?? "";
                var createdAt = EscapeCsvField(item.CreatedAt?.ToString() ?? "");

                csv.AppendLine($"{id},{from},{to},{amount},{converted},{createdAt}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public string GenerateFileName()
        {
            return $"exchange_history_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // If field contains comma, quote, or newline, wrap in quotes and escape quotes
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
        }
    }
}

