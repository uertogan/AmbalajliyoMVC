using System.ComponentModel.DataAnnotations;

namespace AmbalajliyoMVC.ViewModels
{
    /// <summary>
    /// ViewModel representing a user in the Ambalajliyo MVC application.
    /// </summary>
    public class UserViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre hash'i gereklidir.")]
        public string PasswordHash { get; set; }

        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Ad gereklidir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad gereklidir.")]
        public string Surname { get; set; }
        public string AmbalajliyoRoleId { get; set; }

        public bool? EmailConfirmed { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public bool PhoneNumberConfirmed { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;
        public bool LockoutEnabled { get; set; } = false;

        
        public int AccessFailedCount { get; set; } = 0;
    }
}

