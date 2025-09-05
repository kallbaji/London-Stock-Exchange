
namespace LSETradeApi.Models
{
    public class TradeRequest
    {
        public string? TickerSymbol { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string? BrokerId { get; set; }
    }
}
