using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.Services;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace AmbalajliyoMVC.Areas.MusteriHizmetleri.Controllers
{
    [Area("MusteriHizmetleri")]
    [Authorize(Roles = "Müşteri Hizmetleri")]
    public class MusteriHizmetleriFaqController : Controller
    {
        private readonly FaqAPIService _faqAPIService;

        public MusteriHizmetleriFaqController(FaqAPIService faqAPIService)
        {
            _faqAPIService = faqAPIService;
        }

        public async Task<IActionResult> GetAllFaq(int page = 1) // bütün soruları getir
        {
            var faqs = await _faqAPIService.GetAllFaqAsync();
            var pagedItems = faqs.ToPagedList(page,10);
            return View(pagedItems);
        }

        public async Task<IActionResult> CreateFaq()
        {
            //Burası sadece ekleme formunu açmak içindir.
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaq(FaqViewModel faqViewModel)
        {
            try
            {
                await _faqAPIService.CreateFaqAsync(faqViewModel);
                return RedirectToAction(nameof(GetAllFaq)); // ekleme işleminden sonra bütün soruların listelenmesi için GetAllFaq metodunu çağır.
            }
            catch (Exception ex)
            {
                // Eğer bir hata oluştuysa TempData ile hata mesajını taşıyoruz
                TempData["ErrorMessage"] = ex.Message;
                return View(faqViewModel); // Formu hata mesajı ile tekrar render ediyoruz
            }
           
        }

        public async Task<IActionResult> GetByIdFaq(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var faq = await _faqAPIService.GetByIdFaqAsync(id);
           
            return View(faq);
        }

        public async Task<IActionResult> UpdateFaq(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                var faq = await _faqAPIService.GetByIdFaqAsync(id);
                if (faq == null)
                {
                    return NotFound(); // API'den soru alınamadı
                }
                return View(faq); // faq verilerini güncelleme formuna gönder
            }
            catch (Exception ex)
            {
                // Hata durumunda ayrıntılı bilgi ver
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the faq: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFaq(string id, FaqViewModel faqViewModel)
        {
            if (id != faqViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // API'deki update'i çağır. hem id'si var hem de request'in body'si var
                    await _faqAPIService.UpdateFaqAsync(id, faqViewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _faqAPIService.FaqExists(faqViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetAllFaq));
            }
            return View(faqViewModel);
        }

        [HttpPost, ActionName("DeleteFaq")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Emin misiniz ekranından Delete'e basılınca buraya gelir.(postuna)
            //yine o id'ye ait olan soru API'deki metod ile bulunup, o id kullanılarak API'deki silme metodu çağırılır ve silinir.
            var faq = await _faqAPIService.GetByIdFaqAsync(id);
            if (faq != null)
            {
                //API'daki silme metodunu çağıracağız.
                await _faqAPIService.DeleteFaqAsync(id);
            }

            return RedirectToAction(nameof(GetAllFaq));
        }
    }
}
