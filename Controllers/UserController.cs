using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbalajliyoMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserAPIService _userAPIService;
        private readonly RoleAPIService _roleAPIService;

        public UserController(UserAPIService userAPIService, RoleAPIService roleAPIService)
        {
            _userAPIService = userAPIService;
            _roleAPIService = roleAPIService;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userAPIService.GetAllUsersAsync();
            var roles = await _roleAPIService.GetAllRolesAsync();


            ViewBag.Roles = roles;

            return View(users);
        }
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userAPIService.GetUserByIdAsync(id);
            var roles = await _roleAPIService.GetAllRolesAsync();
            HttpContext.Session.SetString("UserLogin", user.IsAdmin.ToString());
            HttpContext.Session.SetString("UserId", user.Id);


            ViewBag.Roles = roles; // Rolleri ViewBag'e aktarıyoruz

            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Name = user.Name,
                Surname = user.Surname,
                AmbalajliyoRoleId = user.AmbalajliyoRoleId // ID'yi View'de kullanmak üzere saklıyoruz
            };

            return View(userViewModel);
        }

        [HttpGet]

        public async Task<IActionResult> Register()
        {
            var model = new UserViewModel();

            // Rolleri API servisi üzerinden alın
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

            // Başarıyla kaydedildikten sonra yönlendirme
            return RedirectToAction("Index");


            // Formda hata varsa, roller verilerini tekrar yükleyin ve kullanıcıyı geri gönderin
            //var roles = await _roleAPIService.GetAllRolesAsync();
            //ViewBag.Roles = roles.Select(role => new { role.Id, role.Name }).ToList();

            //return View(userViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(Guid userId, Guid roleId)
        {
            try
            {
                await _userAPIService.UpdateUserRoleAsync(userId.ToString(), roleId.ToString());
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Rol güncellenirken bir hata oluştu: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST method - Kullanıcı giriş işlemini yapar
        [HttpPost]
        public async Task<IActionResult> Login(string email, string sifre)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre))
            {
                ModelState.AddModelError("", "Kullanıcı adı ve şifre gereklidir.");
                return View();
            }

            // Kullanıcıyı doğrula
            var kullanici = await _userAPIService.LoginAsync(email, sifre);
            if (kullanici == null)
            {
                ModelState.AddModelError("", "Geçersiz kulanıcı adı veya şifre.");
                return View();
            }

            // Kullanıcı bilgilerini oturuma kaydet
            HttpContext.Session.SetString("UserId", kullanici.Id);
            HttpContext.Session.SetString("UserName", kullanici.Name);
            if (kullanici.IsAdmin)
            {
                HttpContext.Session.SetString("UserAdmin", kullanici.Id);
            }
            // Admin olup olmadığını kontrol et

            // Admin paneline yönlendir
            return RedirectToAction("Index");

        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userAPIService.GetUserByIdAsync(userId);

            if (user != null)
            {
                await _userAPIService.DeleteUserAsync(userId, user);
            }

            return RedirectToAction("Index");
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
                    return NotFound(); // User not found
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
                // Return error message
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the user: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(string id, UserViewModel userViewModel, string? newPassword)
        {
            if (id != userViewModel.Id)
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
                        return NotFound(); // User not found
                    }

                    // Update user details
                    user.Name = userViewModel.Name;
                    user.Surname = userViewModel.Surname;
                    user.Email = userViewModel.Email;

                    // Update password only if a new one is provided
                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        // Await the hashing operation
                        user.PasswordHash = await _userAPIService.HashPassword(newPassword);
                    }

                    await _userAPIService.UpdateUserAsync(id, user);

                    return RedirectToAction("GetUserById", new { id = id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency exception
                    throw;
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the user: {ex.Message}");
                }
                
            }
            return View(userViewModel);
        }


        public async Task<IActionResult> GetAllLog() // Bütün logları getir
        {
            return View(await _userAPIService.GetAllLogAsync());
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
            // API'ye logout isteği gönder
            var isLoggedOutFromApi = await _userAPIService.LogoutAsync();

            if (isLoggedOutFromApi)
            {
                // MVC projesinde kullanıcıyı logout yap
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }

            // Eğer API'den logout başarısız olduysa, hata sayfasına yönlendirebilirsiniz
            return View("Error");
        }
    }
}
