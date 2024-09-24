
using System.ComponentModel.DataAnnotations;

namespace AmbalajliyoMVC.ViewModels
{
    /// <summary>
    /// ViewModel for handling category data in the Ambalajliyo MVC application.
    /// </summary>
    public class CategoryViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }
    }
}
