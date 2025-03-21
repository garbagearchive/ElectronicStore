using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly KetNoiCSDL _dbc;

    public AuthService(IConfiguration configuration, KetNoiCSDL dbc)
    {
        _configuration = configuration;
        _dbc = dbc;
    }

    // Đăng ký người dùng
    public async Task<User> Register(User user, string password)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        _dbc.Users.Add(user);
        await _dbc.SaveChangesAsync();
        return user;
    }

    // Đăng nhập người dùng
    public async Task<string> Login(string username, string password)
    {
        var user = await _dbc.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null; // Sai tên đăng nhập hoặc mật khẩu
        }

        // Tạo token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}