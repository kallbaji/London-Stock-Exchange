
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

        

        [HttpGet("stocks/value/{tickerSymbol}")]
        public async Task<IActionResult> GetStockValue(string tickerSymbol)
        {
            var avgPrice = await _context.lsetable
                .Where(t => t.tickersymbol == tickerSymbol)
                .AverageAsync(t => t.price);

            return Ok(new { tickerSymbol, averagePrice = avgPrice.ToString("F2") });
        }

        
    }
}
