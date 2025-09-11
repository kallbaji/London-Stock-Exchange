
using System.Text.Json;
using LSEAuth;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLseJwtAuthentication(builder.Configuration);
builder.Services.AddControllers(); 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(LSEDAL.LSEDAL.ConnectionString));
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            var err = JsonSerializer.Serialize(new { status = "error", message = error.Error.Message });
            await context.Response.WriteAsync(err);
        }
    });
});
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
