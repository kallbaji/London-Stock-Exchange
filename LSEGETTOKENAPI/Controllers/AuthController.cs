using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LSEDAL;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
      private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }
    [HttpPost("token")]
    public async Task<IActionResult> GenerateToken([FromBody] LoginTable model)
    {

       var user = await _context.logintable.FirstOrDefaultAsync(u => u.username == model.username && u.password == model.password);

        if (user != null)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, model.username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A_Very_Long_Random_Secret_Key_1234567890"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "LSEProjectAPI",
                audience: "LSEProjectClient",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        return Unauthorized();
    }
}

