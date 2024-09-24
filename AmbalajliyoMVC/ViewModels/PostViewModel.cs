using System.ComponentModel.DataAnnotations;

namespace AmbalajliyoMVC.ViewModels
{
    public class PostViewModel
    {
        /// <summary>
        /// ViewModel for managing blog posts in the Ambalajliyo MVC application.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Başlık gereklidir.")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter uzunluğunda olabilir.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bilgi gereklidir.")]
        public string Info { get; set; }

       
        public IFormFile? ImageUrl { get; set; }

        public string? Image { get; set; }

        public bool IsPublished { get; set; }
    }
}
