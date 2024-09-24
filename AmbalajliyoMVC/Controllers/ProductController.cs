using AmbalajliyoMVC.Helper;
using AmbalajliyoMVC.Service;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace AmbalajliyoMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductAPIService _productAPIService;
        private readonly CategoryAPIService _categoryAPIService;

        public ProductController(ProductAPIService productAPIService, CategoryAPIService categoryAPIService)
        {
            _productAPIService = productAPIService;
            _categoryAPIService = categoryAPIService;
        }       

        public async Task<IActionResult> FilterProductsByCategory(string categoryId, string categoryName, int page = 1)
        {
            var categories = await _categoryAPIService.GetAllCategoryAsync();
            ViewBag.Categories = categories;

            var products = await _productAPIService.GetProductsByCategoryAsync(categoryId);
            var pagedItems = products.ToPagedList(page, 12); // KAÇ TANE ÜRÜN LİSTELENSİN İSTİYORSAN İKİNCİ PARAMETREYİ DEĞİŞTİR

            return View(pagedItems);
        }

        public async Task<IActionResult> GetByIdProduct(string id, string categoryName, string productName)
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

            var category = await _categoryAPIService.GetByIdCategoryAsync(product.CategoryId);
            categoryName = category.Name.ToSeoFriendly();

            // Ensure the product and category names are in SEO-friendly format and match the route parameters
            if (product.Name.ToSeoFriendly() != productName || categoryName != category.Name.ToSeoFriendly())
            {
                return NotFound();
            }

            return View(product);
        }
    }
}