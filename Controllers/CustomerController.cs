using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbalajliyoMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerAPIService _customerAPIService;

        public CustomerController(CustomerAPIService customerAPIService)
        {
            _customerAPIService = customerAPIService;
        }

        public async Task<IActionResult> GetAllCustomers() // bütün müşterileri getir
        {
            return View(await _customerAPIService.GetAllCustomersAsync());
        }

        public IActionResult İade()
        {
            // Burası sadece ekleme formunu açmak içindir.
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> İade(string id, CustomerViewModel customerViewModel)
        {
            if (id!=null)
            {
                customerViewModel.Title = "İade";
                customerViewModel.ProductId = id;
            }
            
            {
                // Her şey yolunda gittiği zaman formdan gelen müşteri nesnesini, API'deki ekleme metoduna göndereceğiz.
                await _customerAPIService.CreateCustomerAsync(customerViewModel);
                return RedirectToAction("Index","Home"); // ekleme işleminden sonra bütün müşteriler listelensin.
            }
            
        }
        public IActionResult Siparis()
        {
            // Burası sadece ekleme formunu açmak içindir.
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Siparis(string id, CustomerViewModel customerViewModel)
        {
            if (id != null)
            {
                customerViewModel.Title = "Sipariş";
                customerViewModel.ProductId = id;
            }

            {
                // Her şey yolunda gittiği zaman formdan gelen müşteri nesnesini, API'deki ekleme metoduna göndereceğiz.
                await _customerAPIService.CreateCustomerAsync(customerViewModel);
                return RedirectToAction("Index", "Home"); // ekleme işleminden sonra bütün müşteriler listelensin.
            }

        }
        public IActionResult Sikayet()
        {
            // Burası sadece ekleme formunu açmak içindir.
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Sikayet(string id, CustomerViewModel customerViewModel)
        {
            if (id != null)
            {
                customerViewModel.Title = "Şikayet";
                customerViewModel.ProductId = id;
            }

            {
                // Her şey yolunda gittiği zaman formdan gelen müşteri nesnesini, API'deki ekleme metoduna göndereceğiz.
                await _customerAPIService.CreateCustomerAsync(customerViewModel);
                return RedirectToAction("Index", "Home"); // ekleme işleminden sonra bütün müşteriler listelensin.
            }

        }
        public async Task<IActionResult> GetCustomerById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var customer = await _customerAPIService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound(); // API'den müşteri alınamadı
            }

            return View(customer);
        }

        public async Task<IActionResult> UpdateCustomer(string? id)
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
                    return NotFound(); // API'den müşteri alınamadı
                }

                return View(customer); // Müşteri verilerini güncelleme formuna gönder
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the customer: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(string id, CustomerViewModel customerViewModel)
        {
            if (id != customerViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // API'deki update'i çağır.
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
                return RedirectToAction(nameof(GetAllCustomers));
            }
            return View(customerViewModel);
        }

        public async Task<IActionResult> DeleteCustomer(string? id)
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
                    return NotFound(); // API'den müşteri alınamadı
                }

                return View(customer); // emin misiniz ekranına git
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the customer: {ex.Message}");
            }
        }

        [HttpPost, ActionName("DeleteCustomer")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customer = await _customerAPIService.GetCustomerByIdAsync(id);
            if (customer != null)
            {
                await _customerAPIService.DeleteCustomerAsync(id);
            }

            return RedirectToAction(nameof(GetAllCustomers));
        }

        public async Task<IActionResult> GetAllIades() // bütün iadeleri getir
        {
            return View(await _customerAPIService.GetAllIadesAsync());
        }
        public async Task<IActionResult> GetAllSikayets() // bütün şikayetleri getir
        {
            return View(await _customerAPIService.GetAllSikayetsAsync());
        }
        public async Task<IActionResult> GetAllSiparis() // bütün siparişleri getir
        {
            return View(await _customerAPIService.GetAllSiparisAsync());
        }
        public async Task<IActionResult> Metot() // bütün siparişleri getir
        {
            return View(await _customerAPIService.GetAllSiparisAsync());
        }
    }
}
