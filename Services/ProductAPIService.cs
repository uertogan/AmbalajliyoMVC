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

        //public async Task<List<ProductViewModel>> GetProductWithIncludesAsync()
        //{
        //    var response = await _httpClient.GetAsync("GetProductWithIncludes");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var products = await response.Content.ReadAsAsync<List<ProductViewModel>>();
        //        return products;
        //    }
        //    return new List<ProductViewModel>();
        //}

        public async Task CreateProductAsync(ProductViewModel productViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(productViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Product/AddProduct", content);  // Yanıtın başarılı olup olmadığını kontrol edin

                //response hata vermesine rağmen veri tabanına ekleme işlemi yapıyor
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

        public async Task<ProductViewModel> GetByIdProductAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));
            }

            var response = await _httpClient.GetAsync($"Product/GetByIdProduct/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                // Yanıt koduna göre hata işle
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null; // ID'ye sahip kategori bulunamadı
                }

                response.EnsureSuccessStatusCode(); // Diğer hatalar için hata fırlat
            }

            var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();

            if (product == null)
            {
                throw new Exception("Product data could not be deserialized from response.");
            }

            return product;
        }

        public async Task DeleteProductAsync(string productId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Product/DeleteProduct/{productId}");

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

        public async Task UpdateProductAsync(string productId, ProductViewModel productViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(productViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"Product/UpdateProduct/{productId}", content);

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

        public async Task<bool> ProductExists(string productId) // id'ye ait olan ürün var mı kontrol eden ve bool değer döndüren metot.
        {
            var product = await _httpClient.GetFromJsonAsync<ProductViewModel>("Product/GetByIdProduct/" + productId);

            return product != null;
        }
    }
}
