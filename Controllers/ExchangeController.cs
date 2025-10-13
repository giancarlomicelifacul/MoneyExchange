using Cambio.Infra.Repository;
using Cambio.Service;
using Microsoft.AspNetCore.Mvc;

namespace Cambio.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        private readonly ExchangeRateService _service;
        private readonly ExchangeHistoryRepository _repository;

        public ExchangeController(ExchangeRateService service, ExchangeHistoryRepository repository)
        {
            _service = service;
            _repository = repository;
        }

        // GET api/exchange/convert?from=USD&to=BRL&amount=100
        [HttpGet("convert")]
        public async Task<IActionResult> ConvertAsync(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] decimal amount)
        {
            var result = await _service.ConvertAsync(from, to, amount);
            if (result == null)
                return BadRequest(new { message = "Conversion failed" });

            return Ok(result);
        }

        // GET api/exchange/history
        [HttpGet("history")]
        public IActionResult GetHistory(
            [FromQuery] string? fromCurrency,
            [FromQuery] string? toCurrency,
            [FromQuery] decimal? minAmount,
            [FromQuery] decimal? maxAmount,
            [FromQuery] decimal? minConverted,
            [FromQuery] decimal? maxConverted,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var history = _repository.GetHistory(
                fromCurrency,
                toCurrency,
                minAmount,
                maxAmount,
                minConverted,
                maxConverted,
                startDate,
                endDate
            );

            return Ok(history);
        }
    }
}
