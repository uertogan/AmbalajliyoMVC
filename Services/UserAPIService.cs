using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Text.Json;

namespace AmbalajliyoMVC.Service
{
    public class UserAPIService
    {
        private readonly HttpClient _httpClient;


        public UserAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }
        public async Task Register(UserViewModel userViewModel)
        {
            try
            {

                var content = new StringContent(JsonConvert.SerializeObject(userViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("User/AddUser", content);
                // Yanıtın başarılı olup olmadığını kontrol edin response.EnsureSuccessStatusCode(); // Bu satırda hata alıyorsunuz// Diğer işlemler }
                //// API isteğini gönder
                //var response = await _httpClient.PostAsJsonAsync("CreateRole", role.Name);

                response.EnsureSuccessStatusCode();

            }
            catch (HttpRequestException httpRequestException)
            {
                // HTTP isteği ile ilgili bir hata oluştu
                throw new Exception("An error occurred while sending the request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                // Genel hata yönetimi
                throw new Exception("An unexpected error occurred: " + ex.Message);
            }
        }


        // Tüm kullanıcıları al
        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            var response = await _httpClient.GetAsync("User/GetAllUsers");
            if (response.IsSuccessStatusCode)
            {
                var usersJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<UserViewModel>>(usersJson);
            }
            return new List<UserViewModel>();
        }

        // ID ile kullanıcıyı al
        public async Task<UserViewModel> GetUserByIdAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"User/GetUser/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserViewModel>(userJson);
            }
            return null;
        }

        // Kullanıcıya rol ata
        public async Task<bool> AssignRoleToUserAsync(string userId, string roleId)
        {
            // Ensure property names match those expected by the API
            var content = new StringContent(JsonConvert.SerializeObject(new { Id = userId, AmbalajliyoRoleId = roleId }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("UserRole/AssignRoleToUser", content);

            return response.IsSuccessStatusCode;
        }


        // Kullanıcıdan rol kaldır
        public async Task<bool> RemoveRoleFromUserAsync(string userId)
        {
            var response = await _httpClient.DeleteAsync($"User/RemoveRoleFromUser/{userId}");
            return response.IsSuccessStatusCode;
        }

        public async Task UpdateUserRoleAsync(string userId, string roleId)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(roleId), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"UserRole/UpdateUserRole/{userId}", content);

                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                throw new Exception("An error occurred while sending the request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred: " + ex.Message);
            }



        }

        public async Task<UserViewModel> LoginAsync(string email, string sifre)
        {
            try
            {
                var loginData = new { email, sifre };
                var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

                var response = await _httpClient.GetAsync($"User/Login/{email}/{sifre}");

                if (response.IsSuccessStatusCode)
                {
                    var kullaniciJson = await response.Content.ReadAsStringAsync();
                    var kullanici = JsonConvert.DeserializeObject<UserViewModel>(kullaniciJson);
                    return kullanici;
                }
                //else
                //{
                //    var errorContent = await response.Content.ReadAsStringAsync();
                //    Console.WriteLine($"Error Response: {errorContent}");
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

            }
            return null;
        }


        public async Task DeleteUserAsync(string userId, UserViewModel userViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(userViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"User/RemoveUser/{userId}", content);

                // Yanıt başarılıysa işlem tamamlandı
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                // HTTP hataları
                throw new Exception("An error occurred while sending the request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                // Genel hatalar
                throw new Exception("An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task UpdateUserAsync(string userId, UserViewModel userViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(userViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"User/UpdateUser/{userId}", content);

                // Yanıt başarılıysa işlem tamamlandı
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                // HTTP hataları
                throw new Exception("An error occurred while sending the request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                // Genel hatalar
                throw new Exception("An unexpected error occurred: " + ex.Message);
            }
        }


        public async Task<List<LogEntryViewModel>> GetAllLogAsync()
        {
            var response = await _httpClient.GetAsync("User/GetAllLog");
            if (response.IsSuccessStatusCode)
            {
                var logs = await response.Content.ReadAsAsync<List<LogEntryViewModel>>();
                return logs;
            }
            return new List<LogEntryViewModel>();
        }

        public async Task<bool> VerifyPasswordAsync(string id, string sifre)
        {
            var response = await _httpClient.GetAsync($"User/VerifyPassword/{id}/{sifre}");

            return response.IsSuccessStatusCode;
        }

        public async Task<string> HashPassword(string newPassword)
        {
            var response = await _httpClient.GetAsync($"User/HashPassword/{newPassword}");

            if (response.IsSuccessStatusCode)
            {
                // Parse the JSON response
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string to a JsonDocument
                var jsonDocument = JsonDocument.Parse(jsonResponse);

                // Access the "hashedPassword" property
                var hashedPassword = jsonDocument.RootElement.GetProperty("hashedPassword").GetString();

                return hashedPassword;
            }

            return null;
        }

        public async Task<bool> LogoutAsync()
        {
            var response = await _httpClient.PostAsync("User/logout", null);
            return response.IsSuccessStatusCode;
        }

    }
}
