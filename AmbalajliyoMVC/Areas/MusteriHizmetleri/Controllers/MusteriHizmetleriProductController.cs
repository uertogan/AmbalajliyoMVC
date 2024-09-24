using AmbalajliyoMVC.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.Areas.MusteriHizmetleri.Controllers
{
    [Area("MusteriHizmetleri")]
    [Authorize(Roles = "Müşteri Hizmetleri")]
    public class MusteriHizmetleriProductController : Controller
    {
        private readonly ProductAPIService _productAPIService;
        private readonly CategoryAPIService _categoryAPIService;

        public MusteriHizmetleriProductController(ProductAPIService productAPIService, CategoryAPIService categoryAPIService)
        {
            _productAPIService = productAPIService;
            _categoryAPIService = categoryAPIService;
        }
        public async Task<IActionResult> GetByIdProduct(string id)
        {
            var categories = await _categoryAPIService.GetAllCategoryAsync();
            ViewBag.Categories = categories;

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = await _productAPIService.GetByIdProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

    }
}
