using System.ComponentModel.DataAnnotations;

namespace AmbalajliyoMVC.ViewModels
{
    /// <summary>
    /// ViewModel for handling catalog data in the Ambalajliyo MVC application.
    /// </summary>
    public class CatalogViewModel
    {
        public string Id { get; set; } 

        public string? PdfName { get; set; }
       
        public IFormFile PdfUrl{ get; set; }
    }
}
