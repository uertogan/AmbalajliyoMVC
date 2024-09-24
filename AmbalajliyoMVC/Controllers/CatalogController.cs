using AmbalajliyoMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.Controllers
{
    public class CatalogController : Controller
    {
        private readonly CatalogAPIService _catalogAPIService;

        public CatalogController(CatalogAPIService catalogAPIService)
        {
            _catalogAPIService = catalogAPIService;
        }

        public async Task<IActionResult> GetAllCatalog()
        {
            return View(await _catalogAPIService.GetAllCatalogAsync());
        }

        public async Task<IActionResult> GetByIdCatalog(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var category = await _catalogAPIService.GetByIdCatlogAsync(id);
            return View(category);
        }
    }
}