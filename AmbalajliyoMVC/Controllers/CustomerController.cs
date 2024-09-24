using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerAPIService _customerAPIService;
        private readonly ProductAPIService _productAPIService;

        public CustomerController(CustomerAPIService customerAPIService, ProductAPIService productAPIService)
        {
            _customerAPIService = customerAPIService;
            _productAPIService = productAPIService;
        }

        public IActionResult İade()
        {
            // Burası sadece ekleme formunu açmak içindir.
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> İade(string id, CustomerViewModel customerViewModel)
        {
            if (id != null)
            {
                customerViewModel.Id = Guid.NewGuid().ToString();
                customerViewModel.Title = "İade";
                customerViewModel.ProductId = id;
            }
            // Her şey yolunda gittiği zaman formdan gelen müşteri nesnesini, API'deki ekleme metoduna göndereceğiz.
            await _customerAPIService.CreateCustomerAsync(customerViewModel);
            return RedirectToAction("Index", "Home"); // ekleme işleminden sonra bütün müşteriler listelensin.
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
                customerViewModel.Id = Guid.NewGuid().ToString();
                customerViewModel.Title = "Sipariş";
                customerViewModel.ProductId = id;
                //var product = await _productAPIService.GetByIdProductAsync(customerViewModel.ProductId);
                //customerViewModel.Product = product;
            }
            // Her şey yolunda gittiği zaman formdan gelen müşteri nesnesini, API'deki ekleme metoduna göndereceğiz.
            await _customerAPIService.CreateCustomerAsync(customerViewModel);
            return RedirectToAction("Index", "Home"); // ekleme işleminden sonra bütün müşteriler listelensin.
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
                customerViewModel.Id = Guid.NewGuid().ToString();
                customerViewModel.Title = "Şikayet";
                customerViewModel.ProductId = id;
            }
            // Her şey yolunda gittiği zaman formdan gelen müşteri nesnesini, API'deki ekleme metoduna göndereceğiz.
            await _customerAPIService.CreateCustomerAsync(customerViewModel);
            return RedirectToAction("Index", "Home"); // ekleme işleminden sonra bütün müşteriler listelensin.            
        }
    }
}