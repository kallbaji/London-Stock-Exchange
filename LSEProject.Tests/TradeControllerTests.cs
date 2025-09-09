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
using StackExchange.Redis;
namespace LSEProject.Tests
{
    public class TradesControllerTests
    {
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
public async Task PostTrade_DeletesAllValuesByTickersKeys()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase("TestDb_DeleteKeys")
        .Options;
    using var context = new AppDbContext(options);

    var mockCache = new Mock<IDistributedCache>();

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

    var controller = new LSEProject_Controllers.TradesController(context, mockCache.Object, mockRedis.Object);

    var request = new TradeRequest
    {
        TickerSymbol = "AAPL",
        Price = 100,
        Quantity = 10,
        BrokerId = "BRK1"
    };

    // Act
    await controller.PostTrade(request);

    // Assert
    foreach (var key in mockServer.Object.Keys(pattern: "ValuesByTickers_*"))
    {
        mockDb.Verify(db => db.KeyDeleteAsync(key, CommandFlags.None), Times.Once);
    }
}

    }
}