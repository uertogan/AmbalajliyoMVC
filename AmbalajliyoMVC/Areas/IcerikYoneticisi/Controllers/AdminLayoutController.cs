using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.Areas.IcerikYoneticisi.Controllers
{
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
