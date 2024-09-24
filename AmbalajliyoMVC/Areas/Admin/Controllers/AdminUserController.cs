using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;

namespace AmbalajliyoMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]  // JWT token'daki rolü kontrol eder
    public class AdminUserController : Controller
    {
        private readonly UserAPIService _userAPIService;
        private readonly RoleAPIService _roleAPIService;

        public AdminUserController(UserAPIService userAPIService, RoleAPIService roleAPIService)
        {
            _userAPIService = userAPIService;
            _roleAPIService = roleAPIService;
        }

        public async Task<IActionResult> GetAllUsers(int page = 1)
        {
            var users = await _userAPIService.GetAllUsersAsync();
            var roles = await _roleAPIService.GetAllRolesAsync();

            ViewBag.Roles = roles;

            var pagedItems = users.ToPagedList(page, 10);

            return View(pagedItems);
        }

        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userAPIService.GetUserByIdAsync(id);
            var roles = await _roleAPIService.GetAllRolesAsync();

            ViewBag.Roles = roles; // rolleri viewbag'e aktarıyoruz

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Name = user.Name,
                Surname = user.Surname,
                AmbalajliyoRoleId = user.AmbalajliyoRoleId // ID'yi View'da kullanmak üzere saklıyoruz
            };
            return View(userViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var model = new UserViewModel();

            // Rolleri API servisi üzerinden alıyoruz
            var roles = await _roleAPIService.GetAllRolesAsync();
            ViewBag.Roles = roles.Select(role => new RoleViewModel { Id = role.Id, Name = role.Name }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel userViewModel)
        {
            userViewModel.Id = Guid.NewGuid().ToString();

            // Kullanıcıyı API'ye kaydet
            await _userAPIService.Register(userViewModel);
            return RedirectToAction("GetAllUsers");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(Guid userId, Guid roleId)
        {
            try
            {
                await _userAPIService.UpdateUserRoleAsync(userId.ToString(), roleId.ToString());
                return RedirectToAction("GetAllUsers");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Rol güncellenirken bir hata oluştu" + ex.Message);
                return RedirectToAction("GetAllUsers");

            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userAPIService.GetUserByIdAsync(userId);

            if (user != null)
            {
                await _userAPIService.DeleteUserAsync(userId, user);
            }
            return RedirectToAction("GetAllUsers");
        }

        public async Task<IActionResult> UpdateUser(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var user = await _userAPIService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(); // Kullanıcı bulunamadı
                }

                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    UserName = user.UserName,
                    AmbalajliyoRoleId = user.AmbalajliyoRoleId,
                    PasswordHash = user.PasswordHash,
                    EmailConfirmed = user.EmailConfirmed,
                    IsDeleted = user.IsDeleted,
                    IsAdmin = user.IsAdmin,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnabled = user.LockoutEnabled,
                    AccessFailedCount = user.AccessFailedCount
                };

                return View(userViewModel);
            }
            catch (Exception ex)
            {
                // Hata mesajı
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occured while retrieving the user: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(string id, UserViewModel userViewModel, string? newPassword)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userAPIService.GetUserByIdAsync(id);
                    if (user == null)
                    {
                        return NotFound(); // Kullanıcı bulunamadı
                    }

                    // Kullanıcı detaylarını güncelle
                    user.Name = userViewModel.Name;
                    user.Email = userViewModel.Email;
                    user.Surname = userViewModel.Surname;

                    // newPassword girildiyse güncelle
                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        // API'de yeni password'ü hashle
                        user.PasswordHash = await _userAPIService.HashPassword(newPassword);
                    }

                    await _userAPIService.UpdateUserAsync(id, user);
                    return RedirectToAction("GetUserById", new { id = id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the user: {ex.Message}");
                }
            }
            return View(userViewModel);

        }
        public async Task<IActionResult> GetAllLog(int page = 1) // Bütün logları getir
        {
            // Boş bir IPagedList döndür
            var emptyPagedList = new List<LogEntryViewModel>().ToPagedList(page, 20);
            return View(emptyPagedList);
        }

        [HttpPost]
        public async Task<IActionResult> GetAllLog(DateTime? startDate, DateTime? endDate, int page = 1)
        {
            var logs = await _userAPIService.GetAllLogAsync(startDate,endDate);

            TempData["StartDate"] = startDate;
            TempData["EndDate"] = endDate;
            var viewModel = logs.Select(log => new LogEntryViewModel
            {
                TimeStamp = log.TimeStamp,
                Level = log.Level,
                Message = log.Message,
                Exception = log.Exception,
                Properties = log.Properties
            }).ToList();

            var pagedItems = viewModel.ToPagedList(page, 20);

            return View(pagedItems);
        }

        [HttpGet]
        public async Task<IActionResult> PasswordCheck(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                var user = await _userAPIService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(); // API'den kategori alınamadı
                }

                return View(user); // Kategori verilerini güncelleme formuna gönder
            }
            catch (Exception ex)
            {
                // Hata durumunda daha ayrıntılı bilgi ver
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the category: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PasswordCheck(string id, string sifre)
        {
            // Şifreyi doğrulama
            var isPasswordValid = await _userAPIService.VerifyPasswordAsync(id, sifre);

            if (isPasswordValid)
            {
                // Redirect to UpdateUser action if password is valid
                return Json(new { redirectTo = Url.Action("UpdateUser", new { id = id }) });
            }

            // Return JSON indicating invalid password
            return Json(new { isPasswordValid = false });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {

            // Session'dan token ve diğer bilgileri temizle
            HttpContext.Session.Remove("JWToken");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("UserId");

            // Cookie-based kimlik doğrulama varsa oturumu kapat
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Anasayfaya yönlendir
            return RedirectToAction("Index", "Home", new { area = "" });

        }
    }
}
