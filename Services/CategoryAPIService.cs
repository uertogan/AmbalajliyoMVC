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

        public async Task CreateCategoryAsync(CategoryViewModel categoryViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(categoryViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Category/AddCategory", content);  // Yanıtın başarılı olup olmadığını kontrol edin

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

        public async Task<CategoryViewModel> GetByIdCategoryAsync(string? categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                throw new ArgumentException("Category ID cannot be null or empty", nameof(categoryId));
            }

            var response = await _httpClient.GetAsync($"Category/GetByIdCategory/{categoryId}");

            if (!response.IsSuccessStatusCode)
            {
                // Yanıt koduna göre hata işle
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null; // ID'ye sahip kategori bulunamadı
                }

                response.EnsureSuccessStatusCode(); // Diğer hatalar için hata fırlat
            }

            var category = await response.Content.ReadFromJsonAsync<CategoryViewModel>();

            if (category == null)
            {
                throw new Exception("Category data could not be deserialized from response.");
            }

            return category;
        }

        public async Task DeleteCategoryAsync(string categoryId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Category/DeleteCategory/{categoryId}");

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

        public async Task UpdateCategoryAsync(string categoryId, CategoryViewModel categoryViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(categoryViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"Category/UpdateCategory/{categoryId}", content);

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

        public async Task<bool> CategoryExists(string categoryId) // id'ye ait olan kategori var mı kontrol eden ve bool değer döndüren metot.
        {
            var category = await _httpClient.GetFromJsonAsync<CategoryViewModel>("Category/GetByIdCategory/" + categoryId);

            return category != null;
        }


    }
}
