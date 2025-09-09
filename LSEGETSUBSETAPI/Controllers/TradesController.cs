
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
namespace LSEGETSUBSETAPI.Controllers
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

        

        

        

        [HttpPost("stocks/values-by-tickers")]
        public async Task<IActionResult> GetValuesByTickers([FromBody] TickerListRequest request)
        {
            if (request == null || request.Tickers == null || !request.Tickers.Any())
            {
                return BadRequest(new { status = "error", message = "Invalid ticker list" });
            }
            var cacheKey = $"ValuesByTickers_{string.Join("_", request.Tickers)}";
            var cachedResult = await _cache.GetStringAsync(cacheKey);
            if (cachedResult != null)
            {
                return Ok(cachedResult);
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

            var jsonResult = System.Text.Json.JsonSerializer.Serialize(result);
            await _cache.SetStringAsync(cacheKey, jsonResult);

            return Ok(result);  
        }
    }
}
