using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.Extensions.Caching.Distributed;

public class CustomTradesWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public Mock<IDistributedCache> MockCache { get; } = new Mock<IDistributedCache>();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing IDistributedCache registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDistributedCache));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add the mock cache
            services.AddSingleton(MockCache.Object);
        });
    }
}
