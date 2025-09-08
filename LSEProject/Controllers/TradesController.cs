
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
    await _cache.RemoveAsync("ValuesByTickers_*");
            return Ok(new { status = "success", tradeId = trade.id });
        }

        

        
        
    }
}
