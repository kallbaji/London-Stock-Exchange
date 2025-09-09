using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Distributed;
using LSEGETALLAPI_Controllers =  LSEGETALLAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LSEDAL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSEProject_Controllers = LSEPostTradeAPI.Controllers;
using LSEGETTOKENAPI;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using LSEGETAPI_Controllers= LSEGETAPI.Controllers;
using LSEGETSUBSETAPI_controllers = LSEGETSUBSETAPI.Controllers;
using Microsoft.Extensions.DependencyInjection;
namespace LSEProject.Tests
{
    public class TradesControllerTests
    {
        [Fact]
    public async Task GetTradeByTicker_ReturnsOkResult_WithCachedValue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_GETAPI")
            .Options;
        using var context = new AppDbContext(options);

        var mockCache = new Mock<IDistributedCache>();
        var cachedString = "cached trade";
        mockCache.Setup(c => c.GetAsync("StockValue_AAPL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(cachedString));

        var controller = new LSEGETAPI_Controllers.TradesController(context, mockCache.Object);

        // Act
        var result = await controller.GetStockValue("AAPL");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(cachedString, okResult.Value);
    }

   [Fact]
public async Task GetStockValue_WithDbValueAndSetsCache_ReturnsOk()
{
            // Arrange
            var factory = new CustomTradesWebApplicationFactory<LSEGETAPI.Program>();

    var client = factory.CreateClient();

    //Simulate cache miss
   factory.MockCache.Setup(c => c.GetAsync("StockValue_AAPL", It.IsAny<CancellationToken>()))
       .ReturnsAsync((byte[]?)null);

    // Act
    var response = await client.GetAsync("/api/trades/stocks/value/AAPL");

    // Assert
    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

    // Optionally, check the response content
    var json = await response.Content.ReadAsStringAsync();
    Assert.Contains("AAPL", json);
    Assert.Contains("150.00", json);

    // Verify cache was set
    factory.MockCache.Verify(c => c.SetAsync(
       "StockValue_AAPL",
       It.IsAny<byte[]>(),
        It.IsAny<DistributedCacheEntryOptions>(),
       It.IsAny<CancellationToken>()), Times.Once);
}
        [Fact]
    public async Task GetTradesByTickers_ReturnsOkResult_WithCachedValue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_SUBSETAPI")
            .Options;
        using var context = new AppDbContext(options);

        var mockCache = new Mock<IDistributedCache>();
        var cachedString = "cached subset";
        mockCache.Setup(c => c.GetAsync("ValuesByTickers_AAPL_MSFT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(cachedString));

        var controller = new LSEGETSUBSETAPI_controllers.TradesController(context, mockCache.Object);

        // Act
        var result = await controller.GetValuesByTickers(new TickerListRequest() { Tickers = new List<string>(){ "AAPL", "MSFT" } });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(cachedString, okResult.Value);
    }

    [Fact]
    public async Task GetTradesByTickers_ReturnsOkResult_WithDbValue_AndSetsCache()
    {
            // Arrange
            var factory = new CustomTradesWebApplicationFactory<LSEGETSUBSETAPI.Program>();
        var client = factory.CreateClient();
       factory.MockCache.Setup(c => c.GetAsync("ValuesByTickers_AAPL_MSFT", It.IsAny<CancellationToken>()))
           .ReturnsAsync((byte[]?)null);
       factory.MockCache.Setup(c => c.GetAsync("ValuesByTickers_MSFT_AAPL", It.IsAny<CancellationToken>()))
        .ReturnsAsync((byte[]?)null);

            // Act
        var result = await client.PostAsJsonAsync("/api/trades/stocks/values-by-tickers", new TickerListRequest() { Tickers = new List<string>() { "AAPL", "MSFT" } });
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);

        factory.MockCache.Verify(c => c.SetAsync(
            It.Is<string>(key => key == "ValuesByTickers_AAPL_MSFT" || key == "ValuesByTickers_MSFT_AAPL"),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            default), Times.Once);
    }
        [Fact]
        public async Task GetAllStockValues_ReturnsOkResult_WithCachedValue()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            using var context = new AppDbContext(options);

            var mockCache = new Mock<IDistributedCache>();
            var cachedString = "cached result";
            mockCache.Setup(c => c.GetAsync("AllStockValues", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Encoding.UTF8.GetBytes(cachedString));

            var controller = new LSEGETALLAPI_Controllers.TradesController(context, mockCache.Object);

            // Act
            var result = await controller.GetAllStockValues();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(cachedString, okResult.Value);
        }
        [Fact]
        public async Task GetAllStockValues_ReturnsOkResult_WithDbValue_AndSetsCache()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb2")
                .Options;
            using var context = new AppDbContext(options);

            // Seed data
            context.lsetable.AddRange(
                new LSETable { tickersymbol = "AAPL", price = 100, quantity = 10, brokerid = "B1" },
                new LSETable { tickersymbol = "AAPL", price = 200, quantity = 20, brokerid = "B2" },
                new LSETable { tickersymbol = "MSFT", price = 300, quantity = 30, brokerid = "B1" }
            );
            context.SaveChanges();

            var mockCache = new Mock<IDistributedCache>();
            mockCache.Setup(c => c.GetAsync("AllStockValues", It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            var controller = new LSEGETALLAPI_Controllers.TradesController(context, mockCache.Object);

            // Act
            var result = await controller.GetAllStockValues();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);

            mockCache.Verify(c => c.SetAsync(
                "AllStockValues",
                It.IsAny<byte[]>(),
                It.IsAny<Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions>(),
                default), Times.Once);
        }

        [Fact]
        public async Task GetAllStockValues_ReturnsOkResult_WithEmptyList_WhenDbIsEmpty()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb3")
                .Options;
            using var context = new AppDbContext(options);

            var mockCache = new Mock<IDistributedCache>();
            mockCache.Setup(c => c.GetAsync("AllStockValues", It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null); // for cache miss

            var controller = new LSEGETALLAPI_Controllers.TradesController(context, mockCache.Object);

            // Act
            var result = await controller.GetAllStockValues();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Empty(list);
        }
        

        [Fact]
        public async Task PostTrade_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            var appFactory = new WebApplicationFactory<Program>();
            var client = appFactory.CreateClient();

            var trade = new
            {
                TickerSymbol = "AAPL",
                Price = 100,
                Quantity = 10,
                BrokerId = "BRK1"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/trades/trades", trade);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
[Fact]
public async Task PostTrade_WithValidToken_ReturnsOk()
{
            // Arrange

    var factory = new CustomTradesWebApplicationFactory<LSEProject.Program>();
var client = factory.CreateClient();


   var authclient = new CustomAuthWebApplicationFactory<LSEGETTOKENAPI.Program>();

    var login = new { username = "youruser", password = "yourpassword" };
    var tokenResponse = await authclient.CreateClient().PostAsJsonAsync("/api/auth/token", login);
    var tokenObj = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
    var token = tokenObj?.token;

    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
// Mock Redis
            var mockRedis = new Mock<IConnectionMultiplexer>();
            var mockServer = new Mock<IServer>();
            var mockDb = new Mock<IDatabase>();

            // Setup server.Keys to return some fake keys
            var keys = new RedisKey[] { "ValuesByTickers_A", "ValuesByTickers_B" };
            mockServer.Setup(s => s.Keys(It.IsAny<int>(), "ValuesByTickers_*", 10, 0, 0, CommandFlags.None))
                .Returns(keys);

            mockRedis.Setup(r => r.GetServer("localhost:6379", null)).Returns(mockServer.Object);
            mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);

    var trade = new
    {
        TickerSymbol = "AAPL",
        Price = 100,
        Quantity = 10,
        BrokerId = "youruser"
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/trades/trades", trade);

            // Assert
    foreach (var key in mockServer.Object.Keys(pattern: "ValuesByTickers_*"))
            {
                mockDb.Verify(db => db.KeyDeleteAsync(key, CommandFlags.None), Times.Once);
            }
            factory.MockCache.Verify(c => c.RemoveAsync(
                "AllStockValues",
                It.IsAny<CancellationToken>()), Times.Once);
            factory.MockCache.Verify(c => c.RemoveAsync(
                $"StockValue_{trade.TickerSymbol}",
                It.IsAny<CancellationToken>()), Times.Once);
    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
}

[Fact]
public async Task PostTrade_WithInvalidToken_ReturnsUnauthorized()
{
    // Arrange
    var appFactory = new WebApplicationFactory<Program>();
    var client = appFactory.CreateClient();

    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalidtoken");

    var trade = new
    {
        TickerSymbol = "AAPL",
        Price = 100,
        Quantity = 10,
        BrokerId = "BRK1"
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/trades/trades", trade);

    // Assert
    Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
}
        public class TokenResponse
        {
            public required string token { get; set; }
        }
    }
}