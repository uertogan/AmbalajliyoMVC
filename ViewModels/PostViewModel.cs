namespace AmbalajliyoMVC.ViewModels
{
    public class PostViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Info { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public string? Image { get; set; }

        public bool IsPublished { get; set; }
    }
}
