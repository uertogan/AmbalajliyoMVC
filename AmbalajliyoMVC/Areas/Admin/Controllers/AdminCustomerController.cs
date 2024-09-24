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
    public class AdminCustomerController : Controller
    {
        private readonly CustomerAPIService _customerAPIService;
        private readonly ProductAPIService _productAPIService;

        public AdminCustomerController(CustomerAPIService customerAPIService, ProductAPIService productAPIService)
        {
            _customerAPIService = customerAPIService;
            _productAPIService = productAPIService;
        }
       
        public async Task<IActionResult> GetAllIades(int page = 1) // bütün iadeleri getir
        {
            var customers = await _customerAPIService.GetAllCustomersAsync();
            var iades = customers.Where(x => x.Title == "İade").ToList();
            var pagedItems = iades.ToPagedList(page, 10);
            
            ViewBag.Products = await _productAPIService.GetAllProductAsync();

            return View(pagedItems);
        }
        public async Task<IActionResult> GetAllSikayets(int page = 1) // bütün şikayetleri getir
        {
            var customers = await _customerAPIService.GetAllCustomersAsync();
            var sikayets = customers.Where(x => x.Title == "Şikayet").ToList();
            var pagedItems = sikayets.ToPagedList(page,10);

            ViewBag.Products = await _productAPIService.GetAllProductAsync();
            return View(pagedItems);
        }
        public async Task<IActionResult> GetAllSiparis(int page = 1) // bütün siparişleri getir
        {
            var customers = await _customerAPIService.GetAllCustomersAsync();
            var siparis = customers.Where(x => x.Title == "Sipariş").ToList();
            var pagedItems = siparis.ToPagedList(page, 10);

            ViewBag.Products = await _productAPIService.GetAllProductAsync();
            return View(pagedItems);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                var customer = await _customerAPIService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(); // API'den kategori alınamadı
                }
                return View(customer); // faq verilerini güncelleme formuna gönder
            }
            catch (Exception ex)
            {
                // Hata durumunda ayrıntılı bilgi ver
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the category: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id, CustomerViewModel customerViewModel)
        {
            customerViewModel.ModifiedDate = DateTime.Now;
            customerViewModel.IsItAnswered = true;
            if (id != customerViewModel.Id)
            {
                return NotFound();
            }

            try
            {
                // API'deki update'i çağır. hem id'si var hem de request'in body'si var
                await _customerAPIService.UpdateCustomerAsync(id, customerViewModel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _customerAPIService.CustomerExists(customerViewModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (customerViewModel.Title == "Sipariş")
            {
                return RedirectToAction(nameof(GetAllSiparis));
            }
            else if (customerViewModel.Title == "Şikayet")
            {
                return RedirectToAction(nameof(GetAllSikayets));
            }
            else if (customerViewModel.Title == "İade")
            {
                return RedirectToAction(nameof(GetAllIades));
            }
            else
            {
                return View(customerViewModel);
            }
        }
    }
}
