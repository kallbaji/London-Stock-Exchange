
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

        
    }
}
