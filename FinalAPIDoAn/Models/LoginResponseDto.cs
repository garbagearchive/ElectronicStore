namespace FinalAPIDoAn.Models.DTOs
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; } = null!;
        public string Token { get; set; } = null!; // Token JWT
    }
}
