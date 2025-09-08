
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace LSETradeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly AppDbContext _context;

         private readonly IDistributedCache _cache;

        public TradesController(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        

        [HttpGet("stocks/value/{tickerSymbol}")]
        public async Task<IActionResult> GetStockValue(string tickerSymbol)
        {
            if (string.IsNullOrWhiteSpace(tickerSymbol))
            {
                return BadRequest(new { status = "error", message = "Invalid ticker symbol" });
            }

            var cacheKey = $"StockValue_{tickerSymbol}";
            var cachedResult = await _cache.GetStringAsync(cacheKey);
            if (cachedResult != null)
            {
                return Ok(cachedResult);
            }

            var avgPrice = await _context.lsetable
                .Where(t => t.tickersymbol == tickerSymbol)
                .AverageAsync(t => t.price);
 var jsonResult = System.Text.Json.JsonSerializer.Serialize(avgPrice.ToString("F2"));
                await _cache.SetStringAsync(cacheKey, jsonResult);
            
            return Ok(new { tickerSymbol, averagePrice = avgPrice.ToString("F2") });
        }

        
    }
}
