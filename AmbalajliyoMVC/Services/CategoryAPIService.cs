using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace AmbalajliyoMVC.Service
{
    public class CategoryAPIService
    {
        private readonly HttpClient _httpClient;

        public CategoryAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves all category items from the API.
        /// </summary>
        /// <returns>A list of <see cref="CategoryViewModel"/> objects.</returns>
        public async Task<List<CategoryViewModel>> GetAllCategoryAsync()
        {
            var response = await _httpClient.GetAsync("Category/GetAllCategory");
            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadAsAsync<List<CategoryViewModel>>();
                return categories;
            }
            return new List<CategoryViewModel>();
        }

        /// <summary>
        /// Creates a new category by sending a POST request to the API.
        /// </summary>
        /// <param name="categoryViewModel">The category item to be created.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task CreateCategoryAsync(CategoryViewModel categoryViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(categoryViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Category/AddCategory", content);

                // BadRequest (400) gelirse hata mesajını alalım
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorResponse);

                    if (error != null && error.ContainsKey("message"))
                    {
                        throw new Exception(error["message"]); // Hata mesajını fırlat
                    }
                }

                // Diğer hataları kontrol et
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                // HTTP isteği ile ilgili bir hata oluştuysa
                throw new Exception("Bir hata oluştu: " + httpRequestException.Message);
            }
        }

        /// <summary>
        /// Retrieves a specific category item by its ID from the API.
        /// </summary>
        /// <param name="categoryId">The ID of the category item to retrieve.</param>
        /// <returns>The <see cref="CategoryViewModel"/> object with the specified ID, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the category ID is null or empty.</exception>
        /// <exception cref="Exception">Thrown if an error occurs or if the data cannot be deserialized.</exception>
        public async Task<CategoryViewModel> GetByIdCategoryAsync(string? categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                throw new ArgumentException("Category ID cannot be null or empty", nameof(categoryId));
            }

            var response = await _httpClient.GetAsync($"Category/GetByIdCategory/{categoryId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null; // Category not found
                }
                response.EnsureSuccessStatusCode(); // Throw for other errors
            }

            var category = await response.Content.ReadFromJsonAsync<CategoryViewModel>();

            if (category == null)
            {
                throw new Exception("Category data could not be deserialized from response.");
            }

            return category;
        }

        /// <summary>
        /// Deletes a category item by its ID by sending a DELETE request to the API.
        /// </summary>
        /// <param name="categoryId">The ID of the category item to delete.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task DeleteCategoryAsync(string categoryId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Category/DeleteCategory/{categoryId}");
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

        /// <summary>
        /// Updates an existing category item by sending a PUT request to the API.
        /// </summary>
        /// <param name="categoryId">The ID of the category item to update.</param>
        /// <param name="categoryViewModel">The updated category item data.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task UpdateCategoryAsync(string categoryId, CategoryViewModel categoryViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(categoryViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"Category/UpdateCategory/{categoryId}", content);
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

        /// <summary>
        /// Checks if a category exists by sending a GET request to the API.
        /// </summary>
        /// <param name="categoryId">The ID of the category to check.</param>
        /// <returns>True if the category exists; otherwise, false.</returns>
        public async Task<bool> CategoryExists(string categoryId)
        {
            var category = await _httpClient.GetFromJsonAsync<CategoryViewModel>("Category/GetByIdCategory/" + categoryId);
            return category != null;
        }
    }
}
