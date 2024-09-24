using AmbalajliyoMVC.Models;
using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Retrieves all roles from the API.
        /// </summary>
        /// <returns>A list of <see cref="RoleViewModel"/> objects.</returns>
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

        /// <summary>
        /// Retrieves a specific role by its ID from the API.
        /// </summary>
        /// <param name="roleId">The ID of the role to retrieve.</param>
        /// <returns>The <see cref="RoleViewModel"/> object with the specified ID, or null if not found.</returns>
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

        /// <summary>
        /// Creates a new role by sending a POST request to the API.
        /// </summary>
        /// <param name="roleName">The name of the role to be created.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request or if the API returns an error.</exception>
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
                // Handle HTTP request errors
                throw new Exception("An error occurred while sending the request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                // Handle general errors
                throw new Exception("An unexpected error occurred: " + ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing role by sending a PUT request to the API.
        /// </summary>
        /// <param name="role">The updated role data.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task UpdateRoleAsync(RoleViewModel role)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(role.Name), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"Role/UpdateRole/{role.Id}", content);

                // Ensure that the response indicates success
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                // Handle HTTP request errors
                throw new Exception("An error occurred while sending the request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                // Handle general errors
                throw new Exception("An unexpected error occurred: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes a role by its ID by sending a DELETE request to the API.
        /// </summary>
        /// <param name="roleId">The ID of the role to delete.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task DeleteRoleAsync(string roleId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Role/DeleteRole/{roleId}");

                // Ensure that the response indicates success
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                // Handle HTTP request errors
                throw new Exception("An error occurred while sending the request: " + httpRequestException.Message);
            }
            catch (Exception ex)
            {
                // Handle general errors
                throw new Exception("An unexpected error occurred: " + ex.Message);
            }
        }
    }
}
