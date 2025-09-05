
using Microsoft.EntityFrameworkCore;
using LSEDAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // No AddJsonOptions
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(new LSEDAL.LSEDAL().ConnectionString));
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//    app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
