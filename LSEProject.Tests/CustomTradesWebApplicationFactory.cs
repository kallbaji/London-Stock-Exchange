using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;

public class CustomTradesWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public Mock<IDistributedCache> MockCache { get; } = new Mock<IDistributedCache>();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        
        builder.ConfigureServices(services =>
        {
            // Remove all DbContextOptions registrations (for any context)
        var dbContextOptionsDescriptors = services
            .Where(d => d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("DbContextOptions"))
            .ToList();
        foreach (var descriptor1 in dbContextOptionsDescriptors)
            services.Remove(descriptor1);

        // Remove all AppDbContext registrations
        var dbContextDescriptors = services
            .Where(d => d.ServiceType == typeof(AppDbContext))
            .ToList();
        foreach (var descriptor2 in dbContextDescriptors)
            services.Remove(descriptor2);

        // Add AppDbContext with in-memory DB
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestTradeAuthDB");
        });

        // Build the service provider
        var sp = services.BuildServiceProvider();

        // Seed the database
        using (var scope = sp.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
           db.lsetable.AddRange(
                        new LSETable { tickersymbol = "AAPL", price = 150, quantity = 5, brokerid = "B1" },
                        new LSETable { tickersymbol = "MSFT", price = 250, quantity = 8, brokerid = "B2" }
                    ); db.SaveChanges();
        }
            // Remove existing IDistributedCache registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDistributedCache));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add the mock cache
            services.AddSingleton(MockCache.Object);
        });
    }
}
