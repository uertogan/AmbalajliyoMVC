using AmbalajliyoMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.Controllers
{
    public class PostController : Controller
    {
        private readonly PostApiService _postApiService;

        public PostController(PostApiService postApiService)
        {
            _postApiService = postApiService;
        }
      
        public async Task<IActionResult> GetByIdPost(string id, string postTitle)
        {           
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var post = await _postApiService.GetByIdPostAsync(id);
           
            return View(post);
        }       
    }
}