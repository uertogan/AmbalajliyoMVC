using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace AmbalajliyoMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]  // JWT token'daki rolü kontrol eder
    public class AdminRoleController : Controller
    {
        private readonly RoleAPIService _roleAPIService;

        public AdminRoleController(RoleAPIService roleAPIService)
        {
            _roleAPIService = roleAPIService;
        }

        public async Task<IActionResult> GetAllRoles(int page = 1)
        {
            var roles = await _roleAPIService.GetAllRolesAsync();
            var pagedItems = roles.ToPagedList(page, 5);
            return View(pagedItems);
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
                return RedirectToAction("GetAllRoles");
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
            return RedirectToAction("GetAllRoles");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(RoleViewModel roleViewModel)
        {
            await _roleAPIService.DeleteRoleAsync(roleViewModel.Id);
            return RedirectToAction("GetAllRoles");
        }
    }
}
