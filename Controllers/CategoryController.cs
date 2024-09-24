using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace AmbalajliyoMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryAPIService _categoryAPIService;
        private readonly ProductAPIService _productAPIService;

        public CategoryController(CategoryAPIService categoryAPIService, ProductAPIService productAPIService)
        {
            _categoryAPIService = categoryAPIService;
            _productAPIService = productAPIService;
        }

        public async Task<IActionResult> GetAllCategory() // Bütün kategorileri getir
        {
            return View(await _categoryAPIService.GetAllCategoryAsync());
        }

        public IActionResult CreateCategory()
        {
            //Burası sadece ekleme formunu açmak içindir.
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryViewModel categoryViewModel)
        {
            // Her şey yolunda gittiği zaman formdan gelen kategori nesnesini, API'deki ekleme metoduna göndereceğiz. (Gönderirken, request'in body'sinde gönderiyoruz.)
            await _categoryAPIService.CreateCategoryAsync(categoryViewModel);
            return RedirectToAction(nameof(GetAllCategory)); // ekleme işleminden sonra bütün kategoriler listelenmesi için GetAllCategory metodunu çağır.
        }

        public async Task<IActionResult> GetByIdCategory(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            ViewData["Products"] = _productAPIService.GetAllProductAsync(); // kategoriye ait olan ürün adı listesi veya ürün sayısı ?? 
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

                return View(category); // Kategori verilerini güncelleme formuna gönder
            }
            catch (Exception ex)
            {
                // Hata durumunda daha ayrıntılı bilgi ver
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the category: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(string id, CategoryViewModel categoryViewModel)
        {
            //Güncelleme ekranında güncellenecek olan arabanın bilgileri geldikten sonra, yeni bilgiler yazılır ve sonra Save butonuna basıldığı zaman Edit'in POST'una gider. (Yani buraya gelir)
            if (id != categoryViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // API'deki update'i çağır. hem id'si var hem de request'in body'si var.
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

        //public async Task<IActionResult> DeleteCategory(string? id)
        //{
        //    // asp-route-id ile gelen id, API'deki id'ye göre getir metoduna gönderilir. Silinmek istenen araba bulunduktan sonra "emin misiniz" ekranına yönlendirilir.
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {
        //        var category = await _categoryAPIService.GetByIdCategoryAsync(id);

        //        if (category == null)
        //        {
        //            return NotFound(); // API'den kategori alınamadı
        //        }

        //        return View(category); // emin misiniz ekranına git
        //    }
        //    catch (Exception ex)
        //    {
        //        // Hata durumunda daha ayrıntılı bilgi ver
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the category: {ex.Message}");
        //    }
        //}

        [HttpPost, ActionName("DeleteCategory")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Emin misiniz ekranından Delete'e basılınca buraya gelir.(postuna)
            //yine o id'ye ait olan araba API'deki metod ile bulunup, o id kullanılarak API'deki silme metodu çağırılır ve silinir.
            var category = await _categoryAPIService.GetByIdCategoryAsync(id);
            if (category != null)
            {
                //API'daki silme metodunu çağıracağız.

                // DeleteFromJsonAsync ile silme metodunu çağırırsak silme işlemini veritabanında yapar ama json'a çeviremediği için mvc'de hata verir. Bunu düzeltmek için API'deki silme metodumuzda return Ok(silinecekAraba) döndürmeliyiz.
                //await _httpClient.DeleteFromJsonAsync<Araba>("https://localhost:7276/api/Araba/Sil?id=" + id);


                // api'de return edip başka bir yerde bu silinen arabayı kullanmıyacaksak direkt metin döndürüp DeleteAsyns ile silme işlemini yapabiliriz.
                // silinen arabanın kendisini döndürmek yerine metin döndürmek istiyorsak DeletAsync kullanılabilir.
                await _categoryAPIService.DeleteCategoryAsync(id);
            }

            return RedirectToAction(nameof(GetAllCategory));
        }
    }
}
