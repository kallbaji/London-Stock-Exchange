using System;
using System.ComponentModel.DataAnnotations;

namespace LSEProject.Models;

public class TradeRequest
{
    [Required]
    public string TickerSymbol { get; set; }
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(1, int.MaxValue)]
    public decimal Quantity { get; set; }
    [Required]
    public string BrokerId { get; set; }
}
