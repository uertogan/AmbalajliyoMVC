namespace AmbalajliyoMVC.ViewModels
{
    public class ProductViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? CategoryId { get; set; }
    }
}
