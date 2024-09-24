using AmbalajliyoMVC.Services;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace AmbalajliyoMVC.Areas.IcerikYoneticisi.Controllers
{
    [Area("IcerikYoneticisi")]
    public class IcerikYoneticisiPostController : Controller
    {
        private readonly PostApiService _postApiService;

        public IcerikYoneticisiPostController(PostApiService postApiService)
        {
            _postApiService = postApiService;
        }
        public async Task<IActionResult> GetAllPost(int page = 1) // bütün postları getir
        {
            var posts = await _postApiService.GetAllPostAsync();
            var pagedItems = posts.ToPagedList(page, 12);
            return View(pagedItems);
        }
        public async Task<IActionResult> CreatePost()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostViewModel postViewModel)
        {
            postViewModel.IsPublished = true;
            postViewModel.Id = Guid.NewGuid().ToString();
            // Fotoğraf yükleme işlemi
            if (postViewModel.ImageUrl != null)
            {
                var fileName = Path.GetFileName(postViewModel.ImageUrl.FileName);// Dosya adını alır.
                var filePath = Path.Combine("wwwroot", "Images.Posts", fileName);// Dosya yolunu belirler.

                // Dosyayı belirtilen konuma kaydeder.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await postViewModel.ImageUrl.CopyToAsync(stream);
                }
                postViewModel.Image = fileName; // Fotoğraf adını ViewModel'e atar.
            }
            else
            {
                var fileName = "habergörseldefault.jpg";// Dosya adını alır.
                Path.Combine("wwwroot", "Images.Posts", fileName);// Dosya yolunu belirler.
                postViewModel.Image = fileName;
            }
            // Her şey yolunda gittiği zaman formdan gelen post nesnesini, API'deki ekleme metoduna göndereceğiz. (Gönderirken, request'in body'sinde gönderiyoruz.)
            await _postApiService.CreatePostAsync(postViewModel);
            return RedirectToAction(nameof(GetAllPost)); // ekleme işleminden sonra bütün postların listelenmesi için GetAllPost metodunu çağır.

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
                    return NotFound(); // API'den post alınamadı
                }

                return View(product); // Post verilerini güncelleme formuna gönder
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
            if (id != postViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // postu getir
                    var existingPost = await _postApiService.GetByIdPostAsync(id);
                    if (existingPost == null)
                    {
                        return NotFound(); // Post not found
                    }

                    existingPost.Title = postViewModel.Title;
                    existingPost.Info = postViewModel.Info;
                    existingPost.IsPublished = postViewModel.IsPublished;

                    // resmi güncelle
                    if (postViewModel.ImageUrl != null)
                    {
                        // yeni resmi yükle
                        var fileName = Path.GetFileName(postViewModel.ImageUrl.FileName);
                        var filePath = Path.Combine("wwwroot", "Images.Products", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await postViewModel.ImageUrl.CopyToAsync(stream);
                        }
                        existingPost.Image = fileName;
                    }
                    else
                    {
                        // resim seçilmediyse eski resmi tut
                        existingPost.Image = existingPost.Image;
                    }

                    // Update post through API
                    await _postApiService.UpdatePostAsync(id, existingPost);

                    return RedirectToAction(nameof(GetAllPost));
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
                catch (Exception ex)
                {
                    // Handle other exceptions
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the post: {ex.Message}");
                }
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
