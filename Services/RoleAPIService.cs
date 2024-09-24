using AmbalajliyoMVC.Models;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace AmbalajliyoMVC.Service
{
    public class RoleAPIService
    {
        private readonly HttpClient _httpClient;

        public RoleAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
           
        }

        public async Task<List<RoleViewModel>> GetAllRolesAsync()
        {
            var response = await _httpClient.GetAsync("Role/GetAllRoles");
            if (response.IsSuccessStatusCode)
            {
                var roles = await response.Content.ReadAsAsync<List<RoleViewModel>>();
                return roles;
            }
            return new List<RoleViewModel>();
        }
        [HttpPost]
        public async Task<RoleViewModel> GetRoleByIdAsync(string roleId)
        {
            var response = await _httpClient.GetAsync($"Role/GetRoleById/{roleId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<RoleViewModel>();
            }

            // API'den dönen hata mesajını işleyebilirsiniz
            return null;
        }

        public async Task CreateRoleAsync(string roleName)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(roleName), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("Role/CreateRole", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ErrorViewModel>(errorContent);
                    throw new Exception(error.Error);
                }
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

        public async Task UpdateRoleAsync(RoleViewModel role)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(role.Name), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"Role/UpdateRole/{role.Id}", content);

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
        public async Task DeleteRoleAsync(string roleId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Role/DeleteRole/{roleId}");

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
    }
}
