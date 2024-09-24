using AmbalajliyoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace AmbalajliyoMVC.Services
{
    public class CatalogAPIService
    {
        private readonly HttpClient _httpClient;

        public CatalogAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves all catalog items from the API.
        /// </summary>
        /// <returns>A list of <see cref="CatalogViewModel"/> objects.</returns>
        public async Task<List<CatalogViewModel>> GetAllCatalogAsync()
        {
            var response = await _httpClient.GetAsync("Catalog/GetAllCatalog");
            if (response.IsSuccessStatusCode)
            {
                var catalogs = await response.Content.ReadAsAsync<List<CatalogViewModel>>();
                return catalogs;
            }
            return new List<CatalogViewModel>();
        }

        /// <summary>
        /// Creates a new catalog item by sending a POST request to the API.
        /// </summary>
        /// <param name="catalogViewModel">The catalog item to be created.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task CreateCatalogAsync(CatalogViewModel catalogViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(catalogViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Catalog/AddCatalog", content);  // Yanıtın başarılı olup olmadığını kontrol edin
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
                //response hata vermesine rağmen veri tabanına ekleme işlemi yapıyor
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
        /// Retrieves a specific catalog item by its ID from the API.
        /// </summary>
        /// <param name="catalogId">The ID of the catalog item to retrieve.</param>
        /// <returns>The <see cref="CatalogViewModel"/> object with the specified ID, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the catalog ID is null or empty.</exception>
        /// <exception cref="Exception">Thrown if an error occurs or if the data cannot be deserialized.</exception>
        public async Task<CatalogViewModel> GetByIdCatlogAsync(string catalogId)
        {
            if (string.IsNullOrEmpty(catalogId))
            {
                throw new ArgumentException("Catalog ID cannot be null or empty", nameof(catalogId));
            }

            var response = await _httpClient.GetAsync($"Catalog/GetByIdCatalog/{catalogId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null; // Catalog item not found
                }
                response.EnsureSuccessStatusCode(); // Throw for other errors
            }

            var catalog = await response.Content.ReadFromJsonAsync<CatalogViewModel>();

            if (catalog == null)
            {
                throw new Exception("Catalog data could not be deserialized from response.");
            }

            return catalog;
        }

        /// <summary>
        /// Deletes a catalog item by its ID by sending a DELETE request to the API.
        /// </summary>
        /// <param name="catalogId">The ID of the catalog item to delete.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task DeleteCatalogAsync(string catalogId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Catalog/DeleteCatlog/{catalogId}");
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
        /// Updates an existing catalog item by sending a PUT request to the API.
        /// </summary>
        /// <param name="catalogId">The ID of the catalog item to update.</param>
        /// <param name="catalogViewModel">The updated catalog item data.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task UpdateCatalogAsync(string catalogId, CatalogViewModel catalogViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(catalogViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"Catalog/UpdateCatalog/{catalogId}", content);
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
    }
}
