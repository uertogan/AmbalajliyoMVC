using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.Controllers
{
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
