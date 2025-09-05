using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<LSETable> lsetable { get; set; }

    public int InsertTrade(LSETable trade)
    {
        lsetable.Add(trade);
        return SaveChanges();
    }
}

public class LSETable
{
  
    public int id { get; set; }

    
    public required string tickersymbol { get; set; }
    
    public decimal price { get; set; }
    
    public decimal quantity { get; set; }
    
    public required string brokerid { get; set; }
}