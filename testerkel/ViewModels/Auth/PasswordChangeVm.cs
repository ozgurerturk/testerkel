using System.ComponentModel.DataAnnotations;

namespace testerkel.ViewModels.Auth
{
    public class PasswordChangeVm
    {
        [DataType(DataType.Password)]
        [Display(Name = "Eski Şifre")]
        public string CurrentPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        [MinLength(6, ErrorMessage = "Yeni şifre en az 6 karakter olmalı.")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre (Tekrar)")]
        [Compare(nameof(NewPassword), ErrorMessage = "Yeni şifreler eşleşmiyor.")]
        public string NewPasswordConfirm { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
    }
}
