using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Models.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = null!;
    }
}