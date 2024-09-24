using System;
using System.ComponentModel.DataAnnotations;

namespace AmbalajliyoMVC.ViewModels
{
    /// <summary>
    /// ViewModel for managing customer data in the Ambalajliyo MVC application.
    /// </summary>
    public class CustomerViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Başlık 100 karakterden uzun olamaz.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İsim alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "İsim 50 karakterden uzun olamaz.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyisim alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyisim 50 karakterden uzun olamaz.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
        [StringLength(500, ErrorMessage = "Mesaj 500 karakterden uzun olamaz.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Adres alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "Adres 200 karakterden uzun olamaz.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Şehir alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Şehir 100 karakterden uzun olamaz.")]
        public string City { get; set; }

        public bool IsItAnswered { get; set; } = false;

        [Required(ErrorMessage = "Oluşturulma tarihi zorunludur.")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        [Required(ErrorMessage = "Ürün ID alanı zorunludur.")]
        public string ProductId { get; set; }

        public ProductViewModel? Product { get; set; }
    }
}
