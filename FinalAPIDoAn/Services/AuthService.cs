using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
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
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Username and password cannot be null or empty.");
            }

            var user = await _dbc.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null; // Sai tên đăng nhập hoặc mật khẩu
            }

            if (!double.TryParse(_configuration["Jwt:ExpireMinutes"], out double expireMinutes))
            {
                throw new ArgumentException("Invalid value for JWT expiration minutes.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            // Ghi log lỗi hoặc xử lý ngoại lệ
            throw new ApplicationException("An error occurred while logging in.", ex);
        }
    }
}