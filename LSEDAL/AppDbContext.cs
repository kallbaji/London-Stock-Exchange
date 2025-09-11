using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<LSETable> lsetable { get; set; }

    public DbSet<LoginTable> logintable { get; set; }   
}
public class LoginTable
{
    [Key]
    public required string username { get; set; }
    public required  string password { get; set; }
}

public class LSETable
{

    public int id { get; set; }


    public required string tickersymbol { get; set; }

    public decimal price { get; set; }

    public decimal quantity { get; set; }

    public required string brokerid { get; set; }
}
public class TickerListRequest
    {
        public required List<string> Tickers { get; set; }
    }

