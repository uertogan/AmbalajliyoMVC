using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AmbalajliyoMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserAPIService _userAPIService;

        public UserController(UserAPIService userAPIService)
        {
            _userAPIService = userAPIService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST method - Kullanıcı giriş işlemini yapar
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(loginViewModel.Email) || string.IsNullOrEmpty(loginViewModel.Password))
            {
                ModelState.AddModelError("", "Kullanıcı adı ve şifre gereklidir.");
                return View(loginViewModel);
            }

            try
            {
                // API'ye login isteği yap
                var token = await _userAPIService.LoginAsync(loginViewModel);

                if (string.IsNullOrEmpty(token))
                {
                    ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                    return View(loginViewModel);
                }

                // Token'ı parçala
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                // Token'daki claim'leri al
                var claims = jsonToken.Claims.ToList();

                // Claim'lerden username, role ve userId'yi al
                var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                // "userId" özel claim'i almak için:
                var userIdClaim = claims.FirstOrDefault(c => c.Type == "userId")?.Value;

                var isDeletedClaim = claims.FirstOrDefault(c => c.Type == "isDeleted")?.Value;

                // Eğer kullanıcı silinmişse (IsDeleted == true), giriş yapılmasına izin verme
                if (isDeletedClaim == "True")
                {
                    ModelState.AddModelError("", "Bu kullanıcı silinmiş. Giriş yapamazsınız.");
                    return View(loginViewModel);
                }

                HttpContext.Session.SetString("JWToken", token);
                HttpContext.Session.SetString("Role", role);
                HttpContext.Session.SetString("UserId", userIdClaim);

                if (role == "Admin")
                {
                    // Admin paneline yönlendir
                    return RedirectToAction("GetAllUsers", "AdminUser", new { area = "Admin" });
                }
                else if (role == "İçerik Yöneticisi")
                {
                    return RedirectToAction("GetUserById", "IcerikYoneticisiUser", new { area = "IcerikYoneticisi" });
                }
                else if (role == "Müşteri Hizmetleri")
                {
                    return RedirectToAction("GetUserById", "MusteriHizmetleriUser", new { area = "MusteriHizmetleri" });
                }

                return View();
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                ModelState.AddModelError("", "Bir hata oluştu. Lütfen tekrar deneyin.");
                return View(loginViewModel);
            }
        }
    }
}