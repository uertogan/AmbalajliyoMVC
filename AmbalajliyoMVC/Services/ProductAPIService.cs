using AmbalajliyoMVC.ViewModels;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace AmbalajliyoMVC.Service
{
    public class ProductAPIService
    {
        private readonly HttpClient _httpClient;

        public ProductAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves all products from the API.
        /// </summary>
        /// <returns>A list of <see cref="ProductViewModel"/> objects.</returns>
        public async Task<List<ProductViewModel>> GetAllProductAsync()
        {
            var response = await _httpClient.GetAsync("Product/GetAllProduct");
            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadAsAsync<List<ProductViewModel>>();
                return products;
            }
            return new List<ProductViewModel>();
        }

        /// <summary>
        /// Creates a new product by sending a POST request to the API.
        /// </summary>
        /// <param name="productViewModel">The product data to be created.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task CreateProductAsync(ProductViewModel productViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(productViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Product/AddProduct", content);  // Endpoint for adding a product

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
        /// Retrieves a specific product by its ID from the API.
        /// </summary>
        /// <param name="productId">The ID of the product to retrieve.</param>
        /// <returns>The <see cref="ProductViewModel"/> object with the specified ID, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the product ID is null or empty.</exception>
        /// <exception cref="Exception">Thrown if an error occurs or if the data cannot be deserialized.</exception>
        public async Task<ProductViewModel> GetByIdProductAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));
            }

            var response = await _httpClient.GetAsync($"Product/GetByIdProduct/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null; // Product not found
                }

                response.EnsureSuccessStatusCode(); // Throw for other errors
            }

            var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();

            if (product == null)
            {
                throw new Exception("Product data could not be deserialized from response.");
            }

            return product;
        }

        /// <summary>
        /// Retrieves products filtered by category from the API.
        /// </summary>
        /// <param name="categoryId">The ID of the category to filter products by.</param>
        /// <returns>A list of <see cref="ProductViewModel"/> objects that belong to the specified category.</returns>
        public async Task<List<ProductViewModel>> GetProductsByCategoryAsync(string categoryId)
        {
            var response = await _httpClient.GetAsync($"Product/filter-products-bycategory/{categoryId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ProductViewModel>>();
        }

        /// <summary>
        /// Deletes a product by its ID by sending a DELETE request to the API.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task DeleteProductAsync(string productId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Product/DeleteProduct/{productId}");

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
        /// Updates an existing product by sending a PUT request to the API.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="productViewModel">The updated product data.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task UpdateProductAsync(string productId, ProductViewModel productViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(productViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"Product/UpdateProduct/{productId}", content);

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
        /// Checks if a product exists by sending a GET request to the API.
        /// </summary>
        /// <param name="productId">The ID of the product to check.</param>
        /// <returns>True if the product exists; otherwise, false.</returns>
        public async Task<bool> ProductExists(string productId)
        {
            var product = await _httpClient.GetFromJsonAsync<ProductViewModel>("Product/GetByIdProduct/" + productId);
            return product != null;
        }
    }
}
