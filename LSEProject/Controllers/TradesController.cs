
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace LSEPostTradeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly AppDbContext _context;
 private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;

        public TradesController(AppDbContext context, IDistributedCache cache, IConnectionMultiplexer redis)
        {
            _context = context;
            _cache = cache;
            _redis = redis;
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

    await _cache.RemoveAsync("AllStockValues");
    await _cache.RemoveAsync($"StockValue_{request.TickerSymbol}");
    var server = _redis.GetServer("localhost:6379");
    var db = _redis.GetDatabase();

foreach (var key in server.Keys(pattern: "ValuesByTickers_*"))
{
    await db.KeyDeleteAsync(key);
}
            return Ok(new { status = "success", tradeId = trade.id });
        }

        

        
    }
}
