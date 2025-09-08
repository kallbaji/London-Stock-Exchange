
using Microsoft.EntityFrameworkCore;
using LSEDAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // No AddJsonOptions
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(new LSEDAL.LSEDAL().ConnectionString));
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
