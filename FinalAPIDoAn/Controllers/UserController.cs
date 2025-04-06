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

        // PUT: api/User/{userId}/change-password
        /// <summary>
        /// Đổi mật khẩu cho một người dùng cụ thể dựa vào ID.
        /// YÊU CẦU QUYỀN ADMIN.
        /// </summary>
        /// <param name="userId">ID của người dùng cần đổi mật khẩu.</param>
        /// <param name="changePasswordDto">Thông tin mật khẩu cũ và mới.</param>
        /// <response code="204">Đổi mật khẩu thành công.</response>
        /// <response code="400">Mật khẩu cũ không đúng hoặc dữ liệu không hợp lệ.</response>
        /// <response code="401">Chưa xác thực (nếu dùng [Authorize]).</response>
        /// <response code="403">Không có quyền truy cập (ví dụ: không phải Admin).</response>
        /// <response code="404">Không tìm thấy người dùng với ID cung cấp.</response>
        /// <response code="500">Lỗi server.</response>
        [HttpPut("{userId:int}/change-password")] // <-- Route đã bao gồm userId
        // [Authorize(Roles = "Admin")] // <<< --- CỰC KỲ QUAN TRỌNG: Phải thêm phân quyền ở đây
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Nếu [Authorize] được dùng
        [ProducesResponseType(StatusCodes.Status403Forbidden)]     // Nếu user được xác thực nhưng không phải Admin
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword([FromRoute] int userId, [FromBody] ChangePasswordDto changePasswordDto) // <-- Nhận userId từ route
        {
            // Không cần lấy userId từ context nữa

            var user = await _dbc.Users.FindAsync(userId); // <-- Tìm user bằng userId từ route

            if (user == null)
            {
                return NotFound($"Không tìm thấy tài khoản với ID: {userId}.");
            }

            // *** LƯU Ý QUAN TRỌNG VỀ OldPassword ***
            // Nếu chức năng này dành cho Admin đặt lại mật khẩu (reset), Admin sẽ không biết OldPassword.
            // Trong trường hợp đó, bạn nên BỎ qua bước kiểm tra VerifyPassword này.
            // Nếu bạn muốn giữ check này, nó chỉ phù hợp khi user TỰ đổi mk VÀ endpoint này yêu cầu xác thực đúng user đó.
            if (!VerifyPassword(changePasswordDto.OldPassword, user.PasswordHash))
            {
                // Xem xét lại logic này nếu là Admin reset password
                return BadRequest("Mật khẩu cũ không đúng.");
            }
            // *** HẾT LƯU Ý ***

            // Cập nhật mật khẩu mới đã được hash
            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            _dbc.Entry(user).State = EntityState.Modified; // Đánh dấu entity là đã thay đổi

            try
            {
                await _dbc.SaveChangesAsync(); // Lưu thay đổi vào database
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý trường hợp lỗi tương tranh (concurrency exception) nếu cần
                if (!await UserExists(userId)) // Kiểm tra lại xem user còn tồn tại không
                {
                    return NotFound($"Tài khoản với ID: {userId} không còn tồn tại (có thể đã bị xóa).");
                }
                else
                {
                    // Ghi log lỗi tương tranh và ném lại lỗi hoặc trả về lỗi 500
                    // _logger.LogWarning("Lỗi tương tranh khi cập nhật mật khẩu cho user ID {UserId}", userId);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi tương tranh khi cập nhật dữ liệu.");
                }
            }
            catch (Exception ex) // Bắt các lỗi khác khi lưu DB
            {
                // Log lỗi chi tiết (ex)
                // _logger.LogError(ex, "Lỗi không xác định khi đổi mật khẩu cho user ID {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi phía server khi đổi mật khẩu.");
            }

            // Nếu mọi thứ thành công, trả về 204 No Content
            return NoContent();
        }

        // Hàm helper để kiểm tra user tồn tại (có thể dùng trong xử lý lỗi tương tranh)
        private async Task<bool> UserExists(int id)
        {
            return await _dbc.Users.AnyAsync(e => e.UserId == id);
        }

        // ... các actions khác ...


        /// <summary>
        /// Xóa một tài khoản và tất cả dữ liệu liên quan (Orders, Reviews, Cart, Repairs).
        /// Yêu cầu xử lý xóa dữ liệu liên quan trong code vì DB không có ON DELETE CASCADE.
        /// </summary>
        // [Authorize(Roles = "Admin")] // Nên có phân quyền Admin
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Thêm response cho lỗi khác
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Bước 1: Tải User và các dữ liệu liên quan cần xóa cùng
            // Sử dụng Include và ThenInclude để tải dữ liệu cần thiết
            var userToDelete = await _dbc.Users
                .Include(u => u.Orders)         // Tải các Orders của User
                    .ThenInclude(o => o.OrderDetails) // Tải OrderDetails của từng Order đó
                .Include(u => u.ProductReviews) // Tải các ProductReviews của User (ReviewImages sẽ cascade từ đây)
                .Include(u => u.ProductRepairs) // Tải các ProductRepairs của User
                .Include(u => u.ShoppingCarts) // Tải các ShoppingCart items của User
                .FirstOrDefaultAsync(u => u.UserId == id); // Tìm User theo ID

            if (userToDelete == null)
            {
                return NotFound($"Không tìm thấy người dùng với ID: {id}");
            }

            // Sử dụng try-catch để bắt các lỗi không mong muốn trong quá trình xóa phức tạp này
            try
            {
                // Bước 2: Xóa dữ liệu liên quan TRƯỚC KHI xóa User

                // 2.1. Xóa OrderDetails trước (vì OrderDetails -> Orders không cascade)
                // Lặp qua từng Order của User và xóa OrderDetails của Order đó
                foreach (var order in userToDelete.Orders)
                {
                    // Kiểm tra xem OrderDetails đã được load chưa (phòng trường hợp Include lỗi)
                    if (order.OrderDetails != null && order.OrderDetails.Any())
                    {
                        _dbc.OrderDetails.RemoveRange(order.OrderDetails);
                    }
                }
                // 2.2. Xóa Orders của User
                if (userToDelete.Orders.Any())
                    _dbc.Orders.RemoveRange(userToDelete.Orders);

                // 2.3. Xóa ProductReviews của User (ReviewImages sẽ tự động cascade theo schema)
                if (userToDelete.ProductReviews.Any())
                    _dbc.ProductReviews.RemoveRange(userToDelete.ProductReviews);

                // 2.4. Xóa ProductRepairs của User
                if (userToDelete.ProductRepairs.Any())
                    _dbc.ProductRepairs.RemoveRange(userToDelete.ProductRepairs);

                // 2.5. Xóa ShoppingCart items của User
                if (userToDelete.ShoppingCarts.Any())
                    _dbc.ShoppingCarts.RemoveRange(userToDelete.ShoppingCarts);

                // Bước 3: Xóa chính User đó
                _dbc.Users.Remove(userToDelete);

                // Bước 4: Lưu tất cả các thay đổi vào DB trong một transaction
                await _dbc.SaveChangesAsync();

                // Trả về thành công nếu không có lỗi
                return NoContent(); // 204 No Content - Xóa thành công
            }
            catch (Exception ex) // Bắt các lỗi khác có thể xảy ra
            {
                // Log lỗi chi tiết lại (sử dụng một thư viện logging như Serilog, NLog...)
                // Ví dụ: _logger.LogError(ex, "Lỗi khi xóa người dùng ID {UserId} và dữ liệu liên quan.", id);

                // Trả về lỗi Server Error cho client
                return StatusCode(StatusCodes.Status500InternalServerError, $"Đã xảy ra lỗi khi xóa người dùng và dữ liệu liên quan: {ex.Message}");
            }
        }

        // ... các hàm helper khác nếu có ...
    }
}
