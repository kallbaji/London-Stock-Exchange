using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LSEDAL;

public class CustomAuthWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
{
    builder.ConfigureServices(services =>
    {
        // Remove all DbContextOptions registrations (for any context)
        var dbContextOptionsDescriptors = services
            .Where(d => d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("DbContextOptions"))
            .ToList();
        foreach (var descriptor in dbContextOptionsDescriptors)
            services.Remove(descriptor);

        // Remove all AppDbContext registrations
        var dbContextDescriptors = services
            .Where(d => d.ServiceType == typeof(AppDbContext))
            .ToList();
        foreach (var descriptor in dbContextDescriptors)
            services.Remove(descriptor);

        // Add AppDbContext with in-memory DB
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestAuthDB");
        });

        // Build the service provider
        var sp = services.BuildServiceProvider();

        // Seed the database
        using (var scope = sp.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            db.logintable.Add(new LoginTable { username = "youruser", password = "yourpassword" });
            db.SaveChanges();
        }
    });
}
}