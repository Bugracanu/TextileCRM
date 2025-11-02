using System.ComponentModel.DataAnnotations;

namespace TextileCRM.WebUI.Models;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Yeni şifre gereklidir.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı gereklidir.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre (Tekrar)")]
    [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

