
using Microsoft.EntityFrameworkCore;
using LSEDAL;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // No AddJsonOptions
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(LSEDAL.LSEDAL.ConnectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "LSEProject_";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379"));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
