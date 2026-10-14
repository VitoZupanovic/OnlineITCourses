using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.AuthDtos
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long")]
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
