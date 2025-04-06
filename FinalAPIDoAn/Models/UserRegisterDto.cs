using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Models.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)] // Đặt độ dài tối thiểu cho mật khẩu
        public string Password { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [StringLength(15)]
        public string? Phone { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        // Không cần Role ở đây, sẽ gán mặc định hoặc logic khác
    }
}
