using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.Services;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.Areas.IcerikYoneticisi.Controllers
{
    [Area("IcerikYoneticisi")]
    public class IcerikYoneticisiCatalogController : Controller
    {
        private readonly CatalogAPIService _catalogAPIService;

        public IcerikYoneticisiCatalogController(CatalogAPIService catalogAPIService)
        {
            _catalogAPIService = catalogAPIService;
        }
        public async Task<IActionResult> GetAllCatalog()
        {
            return View(await _catalogAPIService.GetAllCatalogAsync());
        }

        public IActionResult CreateCatalog()
        {
            // Burası sadece ekleme formunu açmak içindir
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCatalog(CatalogViewModel catalogViewModel)
        {
            try
            {
                // Dosya yüklenmiş mi kontrol et
                if (catalogViewModel.PdfUrl == null || catalogViewModel.PdfUrl.Length == 0)
                {
                    // Dosya sağlanmadıysa hata mesajı ekle
                    ModelState.AddModelError("PdfUrl", "PDF dosyasını yüklemeniz gerekmektedir.");
                    return View(catalogViewModel);
                }

                var fileName = Path.GetFileName(catalogViewModel.PdfUrl.FileName);
                var filePath = Path.Combine("wwwroot", "Catalog.Pdf", fileName);

                // Dosya adı mevcut mu kontrol et
                if (System.IO.File.Exists(filePath))
                {
                    // Hata mesajını TempData ile taşı
                    TempData["ErrorMessage"] = "Dosya adı zaten mevcut.";
                    return RedirectToAction("CreateCatalog");
                }

                // Katalog oluşturma işlemi
                catalogViewModel.Id = Guid.NewGuid().ToString();

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await catalogViewModel.PdfUrl.CopyToAsync(stream);
                }

                catalogViewModel.PdfName = fileName;

                await _catalogAPIService.CreateCatalogAsync(catalogViewModel);

                return RedirectToAction(nameof(GetAllCatalog));
            }
            catch (Exception ex)
            {
                // Eğer bir hata oluştuysa TempData ile hata mesajını taşıyoruz
                TempData["ErrorMessage"] = ex.Message;
                return View(catalogViewModel); // Formu hata mesajı ile tekrar render ediyoruz
            }
        }



        [HttpPost, ActionName("DeleteCatalog")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Emin misiniz ekranından Delete'e basılınca buraya gelir.(postuna)
            //yine o id'ye ait olan katalog API'deki metod ile bulunup, o id kullanılarak API'deki silme metodu çağırılır ve silinir.
            var catalog = await _catalogAPIService.GetByIdCatlogAsync(id);
            if (catalog != null)
            {
                //API'daki silme metodunu çağıracağız.
                await _catalogAPIService.DeleteCatalogAsync(id);
            }

            return RedirectToAction(nameof(GetAllCatalog));
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
