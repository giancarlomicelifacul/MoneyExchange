using Cambio.Infra.Repository;
using Cambio.Service;
using Microsoft.AspNetCore.Mvc;

namespace Cambio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ExchangeRateService _service;
        private readonly ExchangeHistoryRepository _repository;
        private readonly CsvExportService _csvExportService;

        public HomeController(ExchangeRateService service, ExchangeHistoryRepository repository, CsvExportService csvExportService)
        {
            _service = service;
            _repository = repository;
            _csvExportService = csvExportService;
        }

        // Render conversion page
        public IActionResult Index()
        {
            return View("Index");
        }

        // Process conversion form (calls service directly)
        [HttpPost]
        public async Task<IActionResult> Convert(string fromCurrency, string toCurrency, decimal amount)
        {
            var result = await _service.ConvertAsync(fromCurrency, toCurrency, amount);
            ViewBag.Result = result;
            return View("Index");
        }

        // Render history page
        public IActionResult History(
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

            ViewBag.Filters = new
            {
                fromCurrency,
                toCurrency,
                minAmount,
                maxAmount,
                minConverted,
                maxConverted,
                startDate,
                endDate
            };

            return View(history);
        }

        // Download history as CSV
        [HttpGet]
        public IActionResult DownloadHistory(
            string? fromCurrency,
            string? toCurrency,
            decimal? minAmount,
            decimal? maxAmount,
            decimal? minConverted,
            decimal? maxConverted,
            DateTime? startDate,
            DateTime? endDate)
        {
            var csvBytes = _csvExportService.GenerateHistoryCsv(
                fromCurrency, toCurrency, minAmount, maxAmount,
                minConverted, maxConverted, startDate, endDate
            );

            var fileName = _csvExportService.GenerateFileName();

            return File(csvBytes, "text/csv", fileName);
        }
    }
}
