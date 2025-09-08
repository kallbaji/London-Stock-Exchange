
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LSETradeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TradesController(AppDbContext context)
        {
            _context = context;
        }

        

        

        

        [HttpGet("stocks/values-by-tickers")]
        public async Task<IActionResult> GetValuesByTickers([FromBody] TickerListRequest request)
        {
            if (request == null || request.Tickers == null || !request.Tickers.Any())
            {
                return BadRequest(new { status = "error", message = "Invalid ticker list" });
            }
            var result = await _context.lsetable
                .Where(t => request.Tickers.Contains(t.tickersymbol))
                .GroupBy(t => t.tickersymbol)
                .Select(g => new
                {
                    tickerSymbol = g.Key,
                    averagePrice = g.Average(t => t.price).ToString("F2")
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
