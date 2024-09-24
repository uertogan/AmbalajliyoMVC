using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbalajliyoMVC.Areas.IcerikYoneticisi.Controllers
{
    [Area("IcerikYoneticisi")]
    public class IcerikYoneticisiUserController : Controller
    {
        private readonly UserAPIService _userAPIService;
        private readonly RoleAPIService _roleAPIService;

        public IcerikYoneticisiUserController(UserAPIService userAPIService, RoleAPIService roleAPIService)
        {
            _userAPIService = userAPIService;
            _roleAPIService = roleAPIService;
        }
        public async Task<IActionResult> GetUserById()
        {
            var roles = await _roleAPIService.GetAllRolesAsync();
            var users = await _userAPIService.GetAllUsersAsync();

            ViewBag.Roles = roles; // rolleri viewbag'e aktarıyoruz
            ViewBag.Users = users; // rolleri viewbag'e aktarıyoruz

            return View();
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
                    return NotFound(); // API'den kişi alınamadı
                }

                return View(user); // Kişi verilerini güncelleme formuna gönder
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
