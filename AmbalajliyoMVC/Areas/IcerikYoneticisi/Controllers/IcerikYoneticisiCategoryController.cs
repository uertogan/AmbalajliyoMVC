using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace AmbalajliyoMVC.Areas.IcerikYoneticisi.Controllers
{
    [Area("IcerikYoneticisi")]
    public class IcerikYoneticisiCategoryController : Controller
    {
        private readonly CategoryAPIService _categoryAPIService;
        private readonly ProductAPIService _productAPIService;

        public IcerikYoneticisiCategoryController(CategoryAPIService categoryAPIService, ProductAPIService productAPIService)
        {
            _categoryAPIService = categoryAPIService;
            _productAPIService = productAPIService;
        }
        public async Task<IActionResult> GetAllCategory(int page = 1) // Bütün kategorileri getir
        {
            var categories = await _categoryAPIService.GetAllCategoryAsync();
            var pagedItems = categories.ToPagedList(page, 10);
            return View(pagedItems);
        }

        public IActionResult CreateCategory()
        {
            // Burası sadece ekleme formunu açmak içindir
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryViewModel categoryViewModel)
        {
            try
            {
                await _categoryAPIService.CreateCategoryAsync(categoryViewModel);
                return RedirectToAction(nameof(GetAllCategory)); // Başarılı ekleme sonrası kategorileri listele
            }
            catch (Exception ex)
            {
                // Eğer bir hata oluştuysa TempData ile hata mesajını taşıyoruz
                TempData["ErrorMessage"] = ex.Message;
                return View(categoryViewModel); // Formu hata mesajı ile tekrar render ediyoruz
            }
        }

        public async Task<IActionResult> GetByIdCategory(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            ViewData["Products"] = _productAPIService.GetProductsByCategoryAsync(id); // kategoriye ait olan ürün adı listesi ve ürün sayısı 

            var category = await _categoryAPIService.GetByIdCategoryAsync(id);
            return View(category);
        }

        public async Task<IActionResult> UpdateCategory(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                var category = await _categoryAPIService.GetByIdCategoryAsync(id);
                if (category == null)
                {
                    return NotFound(); // API'den kategori alınamadı
                }
                return View(category); // kategori verilerini güncelleme formuna gönder
            }
            catch (Exception ex)
            {
                // Hata durumunda ayrıntılı bilgi ver
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the category: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(string id, CategoryViewModel categoryViewModel)
        {
            if (id != categoryViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // API'deki update'i çağır. hem id'si var hem de request'in body'si var
                    await _categoryAPIService.UpdateCategoryAsync(id, categoryViewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _categoryAPIService.CategoryExists(categoryViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetAllCategory));
            }
            return View(categoryViewModel);
        }

        [HttpPost, ActionName("DeleteCategory")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Emin misiniz ekranından Delete'e basılınca buraya gelir.(postuna)
            //yine o id'ye ait olan kategori API'deki metod ile bulunup, o id kullanılarak API'deki silme metodu çağırılır ve silinir.
            var category = await _categoryAPIService.GetByIdCategoryAsync(id);
            if (category != null)
            {
                //API'daki silme metodunu çağıracağız.
                await _categoryAPIService.DeleteCategoryAsync(id);
            }

            return RedirectToAction(nameof(GetAllCategory));
        }
    }
}
