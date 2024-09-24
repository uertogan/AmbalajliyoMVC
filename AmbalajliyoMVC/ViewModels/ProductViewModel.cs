using System.ComponentModel.DataAnnotations;

namespace AmbalajliyoMVC.ViewModels
{
    /// <summary>
    /// ViewModel for managing product details in the Ambalajliyo MVC application.
    /// </summary>
    public class ProductViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Ad gereklidir.")]
        [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter uzunluğunda olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Açıklama gereklidir.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Fiyat gereklidir.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat pozitif bir değer olmalıdır.")]
        public decimal Price { get; set; }

        public string? Image { get; set; }

        
        public IFormFile? ImageUrl { get; set; }

        public string? CategoryId { get; set; }
    }
}
