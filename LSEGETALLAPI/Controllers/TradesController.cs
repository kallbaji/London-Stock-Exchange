
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace LSEGETALLAPI.Controllers
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

        

        [HttpGet("stocks/values")]
        public async Task<IActionResult> GetAllStockValues()
        {
            var cacheKey = "AllStockValues";
            var cachedResult = await _cache.GetStringAsync(cacheKey);
            if (cachedResult != null && cachedResult != string.Empty)
            {
                return Ok(cachedResult);
            }

            var result = await _context.lsetable
                .GroupBy(t => t.tickersymbol)
                .Select(g => new
                {
                    tickerSymbol = g.Key,
                    averagePrice = g.Average(t => t.price).ToString("F2")
                })
                .ToListAsync();
            if (result != null)
            {
                var jsonResult = System.Text.Json.JsonSerializer.Serialize(result);
                await _cache.SetStringAsync(cacheKey, jsonResult);
            }
            return Ok(result);
        }

        
    }
}
