using System.ComponentModel.DataAnnotations;

namespace AmbalajliyoMVC.ViewModels
{
    /// <summary>
    /// ViewModel for managing Frequently Asked Questions (FAQ) in the Ambalajliyo MVC application.
    /// </summary>
    public class FaqViewModel
    {
        public string Id { get; set; }=Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Soru alanı zorunludur.")]
        [StringLength(500, ErrorMessage = "Soru 500 karakterden uzun olamaz.")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Cevap alanı zorunludur.")]
        [StringLength(1000, ErrorMessage = "Cevap 1000 karakterden uzun olamaz.")]
        public string Answer { get; set; }
    }
}
