using AmbalajliyoMVC.ViewModels;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        /// <summary>
        /// Registers a new user by sending a POST request to the API.
        /// </summary>
        /// <param name="userViewModel">The user data to be registered.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request or if the API returns an error.</exception>
        public async Task Register(UserViewModel userViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(userViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("User/AddUser", content);

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
        /// Retrieves all users from the API.
        /// </summary>
        /// <returns>A list of <see cref="UserViewModel"/> objects.</returns>
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

        /// <summary>
        /// Retrieves a specific user by its ID from the API.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The <see cref="UserViewModel"/> object with the specified ID, or null if not found.</returns>
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

        /// <summary>
        /// Assigns a role to a user by sending a POST request to the API.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="roleId">The ID of the role to assign.</param>
        /// <returns>True if the assignment was successful, otherwise false.</returns>
        public async Task<bool> AssignRoleToUserAsync(string userId, string roleId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { Id = userId, AmbalajliyoRoleId = roleId }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("UserRole/AssignRoleToUser", content);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Removes a role from a user by sending a DELETE request to the API.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the removal was successful, otherwise false.</returns>
        public async Task<bool> RemoveRoleFromUserAsync(string userId)
        {
            var response = await _httpClient.DeleteAsync($"User/RemoveRoleFromUser/{userId}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Updates a user's role by sending a PUT request to the API.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="roleId">The new role ID.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
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
        /// Logs in a user by sending a POST request to the API.
        /// </summary>
        /// <param name="loginViewModel">The login credentials.</param>
        /// <returns>The JWT token if login is successful, otherwise null.</returns>
        public async Task<string> LoginAsync(LoginViewModel loginViewModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("User/Login", loginViewModel);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    return token;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Hata Yanıtı: {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deletes a user by sending a PUT request to the API.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <param name="userViewModel">The user data.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task DeleteUserAsync(string userId, UserViewModel userViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(userViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"User/RemoveUser/{userId}", content);

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
        /// Updates a user by sending a PUT request to the API.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="userViewModel">The updated user data.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task UpdateUserAsync(string userId, UserViewModel userViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(userViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"User/UpdateUser/{userId}", content);

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
        /// Retrieves all log entries with optional date filtering.
        /// </summary>
        /// <param name="startDate">The start date for filtering logs.</param>
        /// <param name="endDate">The end date for filtering logs.</param>
        /// <returns>A list of <see cref="LogEntryViewModel"/> objects.</returns>
        public async Task<List<LogEntryViewModel>> GetAllLogAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = new List<string>();
            if (startDate.HasValue)
            {
                query.Add($"startDate={startDate.Value.ToString("yyyy-MM-ddTHH:mm:ss")}");
            }
            if (endDate.HasValue)
            {
                query.Add($"endDate={endDate.Value.ToString("yyyy-MM-ddTHH:mm:ss")}");
            }

            var queryString = string.Join("&", query);
            var response = await _httpClient.GetAsync($"User/GetAllLog?{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var logs = await response.Content.ReadAsAsync<List<LogEntryViewModel>>();
                return logs;
            }

            return new List<LogEntryViewModel>();
        }

        /// <summary>
        /// Verifies a user's password by sending a GET request to the API.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="sifre">The password to verify.</param>
        /// <returns>True if the password is correct, otherwise false.</returns>
        public async Task<bool> VerifyPasswordAsync(string id, string sifre)
        {
            var response = await _httpClient.GetAsync($"User/VerifyPassword/{id}/{sifre}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Hashes a new password by sending a GET request to the API.
        /// </summary>
        /// <param name="newPassword">The new password to hash.</param>
        /// <returns>The hashed password if successful, otherwise null.</returns>
        public async Task<string> HashPassword(string newPassword)
        {
            var response = await _httpClient.GetAsync($"User/HashPassword/{newPassword}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                var hashedPassword = jsonDocument.RootElement.GetProperty("hashedPassword").GetString();
                return hashedPassword;
            }

            return null;
        }

        
    }
}
