using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalAPIDoAn.Models; // << Đảm bảo namespace này chứa KetNoiCSDL và User model
using FinalAPIDoAn.Models.DTOs; // Namespace chứa DTOs
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using FinalAPIDoAn.Data;
using System.ComponentModel.DataAnnotations;
// using Microsoft.AspNetCore.Authorization;
// using System.Security.Claims;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;
        private readonly AuthService _authService;

        public AccountController(KetNoiCSDL dbc, AuthService authService)
        {
            _dbc = dbc;
            _authService = authService;
        }

        // -------------------- User Management Endpoints --------------------

        [HttpGet("List")]
        public IActionResult GetAllUserDetails()
        {
            var users = _dbc.Users.ToList();
            return Ok(new { data = users });
        }

        [HttpGet("Search")]
        public IActionResult SearchName([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Invalid search keyword" });

            var results = _dbc.Users.Where(s => s.FullName.Contains(keyword)).ToList();

            if (!results.Any())
                return NotFound(new { message = "No user found matching the keyword." });

            return Ok(new { data = results });
        }

        [HttpGet("Get/{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _dbc.Users.SingleOrDefault(u => u.UserId == id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new { data = user });
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _dbc.Users.SingleOrDefault(o => o.UserId == id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            user.Username = userDto.Username;
            user.PasswordHash = userDto.PasswordHash;
            user.FullName = userDto.FullName;
            user.Email = userDto.Email;
            user.Phone = userDto.Phone;
            user.Address = userDto.Address;
            user.Role = userDto.Role ?? "Buyer";

            _dbc.Users.Update(user);
            _dbc.SaveChanges();

            return Ok(new { message = "User updated successfully.", data = user });
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            using var transaction = _dbc.Database.BeginTransaction();
            try
            {
                var user = _dbc.Users.SingleOrDefault(o => o.UserId == id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                // Xoá ShoppingCart
                var userCartItems = _dbc.ShoppingCarts.Where(c => c.UserId == id).ToList();
                if (userCartItems.Any())
                {
                    _dbc.ShoppingCarts.RemoveRange(userCartItems);
                }

                // Xoá Orders và dữ liệu liên quan
                var userOrders = _dbc.Orders.Where(o => o.UserId == id).ToList();
                if (userOrders.Any())
                {
                    var orderIds = userOrders.Select(o => o.OrderId).ToList();
                    var orderDetails = _dbc.OrderDetails.Where(od => orderIds.Contains(od.OrderId)).ToList();
                    if (orderDetails.Any()) _dbc.OrderDetails.RemoveRange(orderDetails);

                    var payments = _dbc.Payments.Where(p => orderIds.Contains(p.OrderId)).ToList();
                    if (payments.Any()) _dbc.Payments.RemoveRange(payments);

                    var shippings = _dbc.Shippings.Where(s => orderIds.Contains(s.OrderId)).ToList();
                    if (shippings.Any()) _dbc.Shippings.RemoveRange(shippings);

                    _dbc.Orders.RemoveRange(userOrders);
                }

                // Xoá ProductReviews
                var userReviews = _dbc.ProductReviews.Where(o => o.UserId == id).ToList();
                if (userReviews.Any()) _dbc.ProductReviews.RemoveRange(userReviews);

                // Xoá ProductRepairs
                var userRepairs = _dbc.ProductRepairs.Where(o => o.UserId == id).ToList();
                if (userRepairs.Any()) _dbc.ProductRepairs.RemoveRange(userRepairs);

                _dbc.Users.Remove(user);
                _dbc.SaveChanges();
                transaction.Commit();

                return Ok(new { message = "User and related data deleted successfully." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
            }
        }

        // -------------------- Authentication Endpoints --------------------

        // Sử dụng endpoint Register thay cho "Add" để tạo người dùng mới
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var existingUser = await _dbc.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUser != null)
            {
                return Conflict("Username already exists.");
            }

            // Mã hóa mật khẩu
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = hashedPassword,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Role = request.Role ?? "Buyer",
                CreatedAt = DateTime.UtcNow
            };

            _dbc.Users.Add(newUser);
            await _dbc.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully!" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            var token = await _authService.Login(username, password);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(new { Token = token });
        }

        [HttpPost("AssignRoles")]
        public async Task<IActionResult> AssignRoles([FromBody] AssignRoleRequest request)
        {
            var user = await _dbc.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (string.IsNullOrEmpty(request.Role))
            {
                return BadRequest("Role is required");
            }
            user.Role = request.Role;
            await _dbc.SaveChangesAsync();
            return Ok(new { message = "Role assigned successfully!" });
        }
        // PUT: api/account/change-password
        [HttpPut("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user by their ID.
            var user = _dbc.Users.SingleOrDefault(u => u.UserId == dto.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Verify the old password against the stored hashed password.
            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Old password is incorrect." });
            }

            // Validate that the new password and its confirmation match.
            if (dto.NewPassword != dto.ConfirmNewPassword)
            {
                return BadRequest(new { message = "New password and confirmation do not match." });
            }

            // Update the user's password by hashing the new password.
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _dbc.Users.Update(user);
            _dbc.SaveChanges();

            return Ok(new { message = "Password changed successfully." });
        }
    }

    public class ChangePasswordDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmNewPassword { get; set; }
    }
    // DTO cho User Management
    public class UserDto
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Phone { get; set; }

        [Required]
        public required string Address { get; set; }

        public string? Role { get; set; }
    }

    // DTO cho Register (Authentication)
    public class RegisterRequest
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Phone { get; set; }

        [Required]
        public required string Address { get; set; }

        public string? Role { get; set; }
    }

    // DTO cho AssignRoles
    public class AssignRoleRequest
    {
        public int UserId { get; set; }
        [Required]
        public required string Role { get; set; }
    }
}
