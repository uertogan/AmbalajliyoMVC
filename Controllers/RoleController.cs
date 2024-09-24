using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AmbalajliyoMVC.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleAPIService _roleAPIService;

        public RoleController(RoleAPIService roleAPIService)
        {
            _roleAPIService = roleAPIService;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleAPIService.GetAllRolesAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel roleViewModel)
        {
            try
            {
                await _roleAPIService.CreateRoleAsync(roleViewModel.Name);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(roleViewModel);
            }
        }


        [HttpGet]
        public async Task<IActionResult> UpdateRole(string roleId) 
        {
            var roles = await _roleAPIService.GetAllRolesAsync();
            var role = roles.FirstOrDefault(r => r.Id == roleId);

            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(RoleViewModel roleViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(roleViewModel);
            }
            await _roleAPIService.UpdateRoleAsync(roleViewModel);
            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public async Task<IActionResult> DeleteRole(string roleId)
        //{
        //    var roles = await _roleAPIService.GetAllRolesAsync();
        //    var role = roles.FirstOrDefault(x => x.Id == roleId);
        //    if (role == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(role);
        //}

        [HttpPost]
        public async Task<IActionResult> DeleteRole(RoleViewModel roleViewModel)
        {
            await _roleAPIService.DeleteRoleAsync(roleViewModel.Id);
            return RedirectToAction("Index");
        }
    }
}
