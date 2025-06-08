using System.ComponentModel.DataAnnotations;

namespace BookStore.Service.DTOs
{

    public class UpdateProfileDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Kullanılan parola")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni parola")]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Parolayı onayla")]
        [Compare("NewPassword", ErrorMessage = "Parolalar eşleşmiyor.")]
        public string ConfirmNewPassword { get; set; }
    }
}
