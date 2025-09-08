
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

        

        
        
    }
}
