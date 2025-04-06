using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalAPIDoAn.Models; // << Đảm bảo namespace này chứa KetNoiCSDL và User model
using FinalAPIDoAn.Models.DTOs; // Namespace chứa DTOs
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using FinalAPIDoAn.Data;
// using Microsoft.AspNetCore.Authorization;
// using System.Security.Claims;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc; // Sử dụng DbContext của bạn
        // private readonly ITokenService _tokenService;

        public UserController(KetNoiCSDL dbContext /*, ITokenService tokenService */)
        {
            _dbc = dbContext;
            // _tokenService = tokenService;
        }

        // --- Hashing Helpers ---
        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        private bool VerifyPassword(string password, string hashedPassword) { try { return BCrypt.Net.BCrypt.Verify(password, hashedPassword); } catch { return false; } }
        // --- End Hashing Helpers ---

        // POST: api/User/register
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserRegisterDto registerDto)
        {
            if (await _dbc.Users.AnyAsync(u => u.Username == registerDto.Username)) return BadRequest("Username đã tồn tại.");
            if (await _dbc.Users.AnyAsync(u => u.Email == registerDto.Email)) return BadRequest("Email đã tồn tại.");

            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = HashPassword(registerDto.Password),
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                // Role sẽ tự động lấy giá trị DEFAULT 'Customer' từ DB nếu bạn không gán
                // Hoặc bạn có thể gán tường minh nếu muốn: Role = "Customer",
                // CreatedAt sẽ tự động lấy giá trị DEFAULT GETDATE() từ DB nếu không gán
            };

            _dbc.Users.Add(user);
            await _dbc.SaveChangesAsync();

            // Lấy lại thông tin user vừa tạo (bao gồm cả giá trị default từ DB nếu có)
            var createdUser = await _dbc.Users.FindAsync(user.UserId); // Đọc lại để lấy giá trị default nếu cần

            var userDto = new UserDto
            {
                UserId = createdUser.UserId,
                Username = createdUser.Username,
                FullName = createdUser.FullName,
                Email = createdUser.Email,
                Phone = createdUser.Phone,
                Address = createdUser.Address,
                Role = createdUser.Role, // Lấy giá trị Role thực tế từ DB
                CreatedAt = createdUser.CreatedAt // Lấy giá trị CreatedAt thực tế từ DB
            };

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, userDto);
        }

        // POST: api/User/login
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)] // Hoặc UserDto
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var user = await _dbc.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash)) return Unauthorized("Sai tên đăng nhập hoặc mật khẩu.");

            // --- Tạo Token ---
            // var token = _tokenService.CreateToken(user);
            // --- End Tạo Token ---

            var userDto = new UserDto { /* Map properties */ UserId = user.UserId, Username = user.Username, FullName = user.FullName, Email = user.Email, Phone = user.Phone, Address = user.Address, Role = user.Role, CreatedAt = user.CreatedAt };

            // return Ok(new LoginResponseDto { User = userDto, Token = token });
            return Ok(userDto);
        }

        // GET: api/User
        // [Authorize(Roles = "Admin")] // Ví dụ phân quyền
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _dbc.Users
                                    .Select(user => new UserDto { /* Map properties */ UserId = user.UserId, Username = user.Username, FullName = user.FullName, Email = user.Email, Phone = user.Phone, Address = user.Address, Role = user.Role, CreatedAt = user.CreatedAt })
                                    .ToListAsync();
            return Ok(users);
        }

        // GET: api/User/{id}
        // [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _dbc.Users.FindAsync(id);
            if (user == null) return NotFound();

            var userDto = new UserDto { /* Map properties */ UserId = user.UserId, Username = user.Username, FullName = user.FullName, Email = user.Email, Phone = user.Phone, Address = user.Address, Role = user.Role, CreatedAt = user.CreatedAt };
            return Ok(userDto);
        }

        // PUT: api/User/change-password
        // [Authorize]
        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var userIdString = "1"; // <<< !!! Lấy User ID từ context (ví dụ: Token)
                                    // var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !int.TryParse(userIdString, out var userId)) return Unauthorized();

            var user = await _dbc.Users.FindAsync(userId);
            if (user == null) return NotFound("Không tìm thấy tài khoản.");
            if (!VerifyPassword(changePasswordDto.OldPassword, user.PasswordHash)) return BadRequest("Mật khẩu cũ không đúng.");

            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            _dbc.Entry(user).State = EntityState.Modified;
            await _dbc.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/User/{id}
        // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _dbc.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Lưu ý: Schema mới không có ON DELETE CASCADE cho FK từ Orders, Reviews,... đến Users.
            // Việc xóa User có thể gây lỗi nếu có dữ liệu liên quan trong các bảng khác.
            // Cần xử lý dữ liệu liên quan trước khi xóa User hoặc cấu hình lại FK trong DB.
            try
            {
                _dbc.Users.Remove(user);
                await _dbc.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex) // Bắt lỗi FK Constraint
            {
                // Log lỗi (ex)
                return BadRequest("Không thể xóa người dùng này vì có dữ liệu liên quan (ví dụ: đơn hàng, đánh giá...). Hãy xử lý dữ liệu liên quan trước.");
            }
        }
    }
}
