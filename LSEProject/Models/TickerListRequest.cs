
using System.Collections.Generic;

namespace LSETradeApi.Models
{
    public class TickerListRequest
    {
        public required List<string> Tickers { get; set; }
    }
}
