using AmbalajliyoMVC.Helper;
using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> GetAllProduct() // bütün ürünleri getir
        {
            var categories = await _categoryAPIService.GetAllCategoryAsync();
            ViewBag.Categories = categories;

            return View(await _productAPIService.GetAllProductAsync());
        }
        public async Task<IActionResult> CreateProduct()
        {
            var categories = await _categoryAPIService.GetAllCategoryAsync();
            ViewBag.Categories = categories;
            //Burası sadece ekleme formunu açmak içindir.
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductViewModel productViewModel)
        {
            // Fotoğraf yükleme işlemi
            if (productViewModel.ImageUrl != null)
            {
                var fileName = Path.GetFileName(productViewModel.ImageUrl.FileName);// Dosya adını alır.
                var filePath = Path.Combine("wwwroot", "Images.Products", fileName);// Dosya yolunu belirler.

                // Dosyayı belirtilen konuma kaydeder.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await productViewModel.ImageUrl.CopyToAsync(stream);
                }
                productViewModel.Image = fileName; // Fotoğraf adını ViewModel'e atar.
            }
            else
            {
                var fileName = "ürüngörseldefault.png";// Dosya adını alır.
                Path.Combine("wwwroot", "Images.Products", fileName);// Dosya yolunu belirler.
                productViewModel.Image = fileName;
            }
            // Her şey yolunda gittiği zaman formdan gelen ürün nesnesini, API'deki ekleme metoduna göndereceğiz. (Gönderirken, request'in body'sinde gönderiyoruz.)
            await _productAPIService.CreateProductAsync(productViewModel);
            return RedirectToAction(nameof(GetAllProduct)); // ekleme işleminden sonra bütün kategoriler listelenmesi için GetAllProduct metodunu çağır.

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

            if (product == null || product.Name.ToSeoFriendly() != productName || !categories.Any(c => c.Name.ToSeoFriendly() == categoryName))
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(string? id)
        {
            var categories = await _categoryAPIService.GetAllCategoryAsync();
            ViewBag.Categories = categories;

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var product = await _productAPIService.GetByIdProductAsync(id);

                if (product == null)
                {
                    return NotFound(); // API'den kategori alınamadı
                }

                return View(product); // Ürün verilerini güncelleme formuna gönder
            }
            catch (Exception ex)
            {
                // Hata durumunda daha ayrıntılı bilgi ver
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the category: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(string id, ProductViewModel productViewModel)
        {
            //Güncelleme ekranında güncellenecek olan arabanın bilgileri geldikten sonra, yeni bilgiler yazılır ve sonra Save butonuna basıldığı zaman Edit'in POST'una gider. (Yani buraya gelir)
            if (id != productViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fotoğraf yükleme işlemi
                    if (productViewModel.ImageUrl != null)
                    {
                        var fileName = Path.GetFileName(productViewModel.ImageUrl.FileName);// Dosya adını alır.
                        var filePath = Path.Combine("wwwroot", "Images.Products", fileName);// Dosya yolunu belirler.

                        // Dosyayı belirtilen konuma kaydeder.
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await productViewModel.ImageUrl.CopyToAsync(stream);
                        }
                        productViewModel.Image = fileName; // Fotoğraf adını ViewModel'e atar.
                    }
                    // API'deki update'i çağır. hem id'si var hem de request'in body'si var.
                    await _productAPIService.UpdateProductAsync(id, productViewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _productAPIService.ProductExists(productViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetAllProduct));
            }
            return View(productViewModel);
        }

        //public async Task<IActionResult> DeleteProduct(string? id)
        //{
        //    var categories = await _categoryAPIService.GetAllCategoryAsync();
        //    ViewBag.Categories = categories;

        //    // asp-route-id ile gelen id, API'deki id'ye göre getir metoduna gönderilir. Silinmek istenen araba bulunduktan sonra "emin misiniz" ekranına yönlendirilir.
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {
        //        var product = await _productAPIService.GetByIdProductAsync(id);

        //        if (product == null)
        //        {
        //            return NotFound(); // API'den kategori alınamadı
        //        }

        //        return View(product); // emin misiniz ekranına git
        //    }
        //    catch (Exception ex)
        //    {
        //        // Hata durumunda daha ayrıntılı bilgi ver
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the category: {ex.Message}");
        //    }
        //}

        [HttpPost, ActionName("DeleteProduct")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Emin misiniz ekranından Delete'e basılınca buraya gelir.(postuna)
            //yine o id'ye ait olan araba API'deki metod ile bulunup, o id kullanılarak API'deki silme metodu çağırılır ve silinir.
            var product = await _productAPIService.GetByIdProductAsync(id);
            if (product != null)
            {
                //API'daki silme metodunu çağıracağız.

                // DeleteFromJsonAsync ile silme metodunu çağırırsak silme işlemini veritabanında yapar ama json'a çeviremediği için mvc'de hata verir. Bunu düzeltmek için API'deki silme metodumuzda return Ok(silinecekAraba) döndürmeliyiz.
                //await _httpClient.DeleteFromJsonAsync<Araba>("https://localhost:7276/api/Araba/Sil?id=" + id);


                // api'de return edip başka bir yerde bu silinen arabayı kullanmıyacaksak direkt metin döndürüp DeleteAsyns ile silme işlemini yapabiliriz.
                // silinen arabanın kendisini döndürmek yerine metin döndürmek istiyorsak DeletAsync kullanılabilir.
                await _productAPIService.DeleteProductAsync(id);
            }

            return RedirectToAction(nameof(GetAllProduct));
        }

    }
}
