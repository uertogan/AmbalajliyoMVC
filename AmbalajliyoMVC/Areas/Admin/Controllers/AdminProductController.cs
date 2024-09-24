using AmbalajliyoMVC.Helper;
using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace AmbalajliyoMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]  // JWT token'daki rolü kontrol eder
    public class AdminProductController : Controller
    {
        private readonly ProductAPIService _productAPIService;
        private readonly CategoryAPIService _categoryAPIService;

        public AdminProductController(ProductAPIService productAPIService, CategoryAPIService categoryAPIService)
        {
            _productAPIService = productAPIService;
            _categoryAPIService = categoryAPIService;
        }

        public async Task<IActionResult> GetAllProduct(int page = 1) // bütün ürünleri getir
        {
            var categories = await _categoryAPIService.GetAllCategoryAsync();
            ViewBag.Categories = categories;
            var allProducts = await _productAPIService.GetAllProductAsync();
            var pagedItems = allProducts.ToPagedList(page, 12);

            return View(pagedItems);
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
            //Güncelleme ekranında güncellenecek olan ürünün bilgileri geldikten sonra, yeni bilgiler yazılır ve sonra Save butonuna basıldığı zaman Edit'in POST'una gider. (Yani buraya gelir)
            if (id != productViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _productAPIService.GetByIdProductAsync(id);

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
                        productViewModel.Image = existingProduct.Image;
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


        [HttpPost, ActionName("DeleteProduct")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Emin misiniz ekranından Delete'e basılınca buraya gelir.(postuna)
            //yine o id'ye ait olan ürün API'deki metod ile bulunup, o id kullanılarak API'deki silme metodu çağırılır ve silinir.
            var product = await _productAPIService.GetByIdProductAsync(id);
            if (product != null)
            {
                //API'daki silme metodunu çağıracağız.
                await _productAPIService.DeleteProductAsync(id);
            }

            return RedirectToAction(nameof(GetAllProduct));
        }
    }
}
