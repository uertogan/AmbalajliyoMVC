using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.Services;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbalajliyoMVC.Controllers
{
    public class PostController : Controller
    {
        private readonly PostApiService _postApiService;

        public PostController(PostApiService postApiService)
        {
            _postApiService = postApiService;
        }
        public async Task<IActionResult> GetAllPost() // bütün ürünleri getir
        {
            

            return View(await _postApiService.GetAllPostAsync());
        }
        public async Task<IActionResult> CreatePost()
        {
            
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostViewModel postViewModel)
        {
            postViewModel.IsPublished= true;
            postViewModel.Id=Guid.NewGuid().ToString();
            // Fotoğraf yükleme işlemi
            if (postViewModel.ImageUrl != null)
            {
                var fileName = Path.GetFileName(postViewModel.ImageUrl.FileName);// Dosya adını alır.
                var filePath = Path.Combine("wwwroot", "Images.Products", fileName);// Dosya yolunu belirler.

                // Dosyayı belirtilen konuma kaydeder.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postViewModel.ImageUrl.CopyToAsync(stream);
                }
                postViewModel.Image = fileName; // Fotoğraf adını ViewModel'e atar.
            }
            else
            {
                var fileName = "ürüngörseldefault.png";// Dosya adını alır.
                Path.Combine("wwwroot", "Images.Products", fileName);// Dosya yolunu belirler.
                postViewModel.Image = fileName;
            }
            // Her şey yolunda gittiği zaman formdan gelen ürün nesnesini, API'deki ekleme metoduna göndereceğiz. (Gönderirken, request'in body'sinde gönderiyoruz.)
            await _postApiService.CreatePostAsync(postViewModel);
            return RedirectToAction(nameof(GetAllPost)); // ekleme işleminden sonra bütün kategoriler listelenmesi için GetAllProduct metodunu çağır.

        }
        public async Task<IActionResult> GetByIdPost(string id)
        {
           
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var post = await _postApiService.GetByIdPostAsync(id);
            return View(post);
        }
        [HttpGet]
        public async Task<IActionResult> UpdatePost(string? id)
        {
        

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var product = await _postApiService.GetByIdPostAsync(id);

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
        public async Task<IActionResult> UpdatePost(string id, PostViewModel postViewModel)
        {
            //Güncelleme ekranında güncellenecek olan arabanın bilgileri geldikten sonra, yeni bilgiler yazılır ve sonra Save butonuna basıldığı zaman Edit'in POST'una gider. (Yani buraya gelir)
            if (id != postViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fotoğraf yükleme işlemi
                    if (postViewModel.ImageUrl != null)
                    {
                        var fileName = Path.GetFileName(postViewModel.ImageUrl.FileName);// Dosya adını alır.
                        var filePath = Path.Combine("wwwroot", "Images.Products", fileName);// Dosya yolunu belirler.

                        // Dosyayı belirtilen konuma kaydeder.
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await postViewModel.ImageUrl.CopyToAsync(stream);
                        }
                        postViewModel.Image = fileName; // Fotoğraf adını ViewModel'e atar.
                    }
                    // API'deki update'i çağır. hem id'si var hem de request'in body'si var.
                    await _postApiService.UpdatePostAsync(id, postViewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _postApiService.PostExists(postViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetAllPost));
            }
            return View(postViewModel);
        }
        [HttpPost, ActionName("DeletePost")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            
            var post = await _postApiService.GetByIdPostAsync(id);
            if (post != null)
            {
               
                await _postApiService.DeletePostAsync(id);
            }

            return RedirectToAction(nameof(GetAllPost));
        }
    }
}
