
using Microsoft.EntityFrameworkCore;
using LSEDAL;
using LSEAuth;
 
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLseJwtAuthentication(builder.Configuration);
builder.Services.AddControllers(); // No AddJsonOptions
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(LSEDAL.LSEDAL.ConnectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; 
    options.InstanceName = "LSEProject_";
});
var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
