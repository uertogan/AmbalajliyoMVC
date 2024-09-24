using AmbalajliyoMVC.Models;
using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AmbalajliyoMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductAPIService _productAPIService;
        private readonly CategoryAPIService _categoryAPIService;
        private readonly PostApiService _postApiService;

        public HomeController(ILogger<HomeController> logger ,ProductAPIService productAPIService, CategoryAPIService categoryAPIService,PostApiService postApiService)
        {
            _logger = logger;
            _productAPIService = productAPIService;
            _categoryAPIService = categoryAPIService;
            _postApiService = postApiService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryAPIService.GetAllCategoryAsync();
            ViewBag.Categories = categories;

            var posts = await _postApiService.GetAllPostAsync();
            ViewBag.Posts = posts;

            var products = await _productAPIService.GetAllProductAsync();
            return View(products);
        }

        public async Task<IActionResult> Hakkimizda()
        {
            return View();
        }
        public async Task<IActionResult> MisyonVizyonDegerlerimiz()
        {
            return View();
        }
        //public async Task<IActionResult> Yuvarlak()
        //{
        //    var categories = await _categoryAPIService.GetAllCategoryAsync();
        //    ViewBag.Categories = categories;
        //    return View(await _productAPIService.GetAllProductAsync());
        //}
        //public async Task<IActionResult> Koseli()
        //{
        //    var categories = await _categoryAPIService.GetAllCategoryAsync();
        //    ViewBag.Categories = categories;
        //    return View(await _productAPIService.GetAllProductAsync());
        //}
        //public async Task<IActionResult> Diger()
        //{
        //    var categories = await _categoryAPIService.GetAllCategoryAsync();
        //    ViewBag.Categories = categories;
        //    return View(await _productAPIService.GetAllProductAsync());
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       

    }
}
