using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Models.DTOs
{
    public class UserLoginDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
