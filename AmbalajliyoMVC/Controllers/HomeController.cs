using AmbalajliyoMVC.Models;
using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AmbalajliyoMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductAPIService _productAPIService;
        private readonly CategoryAPIService _categoryAPIService;
        private readonly PostApiService _postApiService;
        private readonly FaqAPIService _faqAPIService;

        public HomeController(ProductAPIService productAPIService, CategoryAPIService categoryAPIService,PostApiService postApiService, FaqAPIService faqAPIService)
        {
            _productAPIService = productAPIService;
            _categoryAPIService = categoryAPIService;
            _postApiService = postApiService;
            _faqAPIService = faqAPIService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryAPIService.GetAllCategoryAsync();
            ViewBag.Categories = categories;
            var post= await _postApiService.GetAllPostAsync();
            ViewBag.Posts = post;    
            return View(await _productAPIService.GetAllProductAsync());
        }

        public async Task<IActionResult> Hakkimizda()
        {
            return View();
        }

        public async Task<IActionResult> MisyonVizyonDegerlerimiz()
        {
            return View();
        }

        public async Task<IActionResult> GetAllFaq() //Sýkça Sorulan Sorularý getir
        {
            var faqs = await _faqAPIService.GetAllFaqAsync();
            return View(faqs);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			var errorViewModel = new ErrorViewModel
			{
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
				// Optionally include an Error message if needed
				// Error = "A specific error message or additional info."
			};
			return View(errorViewModel);
		}
	}
}