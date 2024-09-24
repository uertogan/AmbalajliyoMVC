using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]  // JWT token'daki rolü kontrol eder
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
