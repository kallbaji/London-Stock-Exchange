
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LSETradeApi.Models;

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

        [HttpPost]
        public async Task<IActionResult> PostTrade([FromBody] TradeRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.TickerSymbol) ||
                string.IsNullOrWhiteSpace(request.BrokerId) ||
                request.Price <= 0 ||
                request.Quantity <= 0)
            {
                return BadRequest(new { status = "error", message = "Invalid trade data" });
            }
            var trade = new LSETable
            {
             
                tickersymbol = request.TickerSymbol,
                price = request.Price,
                quantity = request.Quantity,
                brokerid = request.BrokerId
            
            };

        _context.lsetable.Add(trade);
            await _context.SaveChangesAsync();

            return Ok(new { status = "success", tradeId = trade.id });
        }

        [HttpGet("stocks/value/{tickerSymbol}")]
        public async Task<IActionResult> GetStockValue(string tickerSymbol)
        {
            var avgPrice = await _context.lsetable
                .Where(t => t.tickersymbol == tickerSymbol)
                .AverageAsync(t => t.price);

            return Ok(new { tickerSymbol, averagePrice = avgPrice.ToString("F2") });
        }

        [HttpGet("stocks/values")]
        public async Task<IActionResult> GetAllStockValues()
        {
            var result = await _context.lsetable
                .GroupBy(t => t.tickersymbol)
                .Select(g => new
                {
                    tickerSymbol = g.Key,
                    averagePrice = g.Average(t => t.price).ToString("F2")
                })
                .ToListAsync();

            return Ok(result);
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
