using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.Areas.MusteriHizmetleri.Controllers
{
    [Area("MusteriHizmetleri")]
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
