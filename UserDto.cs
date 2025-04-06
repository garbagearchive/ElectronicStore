namespace FinalAPIDoAn.Models.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; } // Sẽ lấy từ User.Role (dựa trên model bạn cung cấp)
                                          // Nếu sửa model theo RoleID, bạn cần join để lấy RoleName ở đây
        public DateTime? CreatedAt { get; set; }
    }
}
