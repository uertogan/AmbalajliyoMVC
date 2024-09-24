using AmbalajliyoMVC.ViewModels;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace AmbalajliyoMVC.Services
{
    public class PostApiService
    {
        private readonly HttpClient _httpClient;

        public PostApiService(HttpClient httpClient)
        {
            
            _httpClient = httpClient;
        }
        public async Task<List<PostViewModel>> GetAllPostAsync()
        {
            var response = await _httpClient.GetAsync("Posts/GetAllPost");
            if (response.IsSuccessStatusCode)
            {
                var posts = await response.Content.ReadAsAsync<List<PostViewModel>>();
                return posts;
            }
            return new List<PostViewModel>();
        }
        public async Task CreatePostAsync(PostViewModel postViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(postViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Posts/AddPost", content);  // Yanıtın başarılı olup olmadığını kontrol edin

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
        public async Task<PostViewModel> GetByIdPostAsync(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                throw new ArgumentException("Product ID cannot be null or empty", nameof(postId));
            }

            var response = await _httpClient.GetAsync($"Posts/GetByIdPost/{postId}");

            if (!response.IsSuccessStatusCode)
            {
                // Yanıt koduna göre hata işle
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null; // ID'ye sahip kategori bulunamadı
                }

                response.EnsureSuccessStatusCode(); // Diğer hatalar için hata fırlat
            }

            var post = await response.Content.ReadFromJsonAsync<PostViewModel>();

            if (post == null)
            {
                throw new Exception("Product data could not be deserialized from response.");
            }

            return post;
        }
        public async Task DeletePostAsync(string postId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Posts/DeletePost/{postId}");

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
        public async Task UpdatePostAsync(string postId, PostViewModel postViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(postViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"Posts/UpdatePost/{postId}", content);

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
        public async Task<bool> PostExists(string postId) // id'ye ait olan ürün var mı kontrol eden ve bool değer döndüren metot.
        {
            var post = await _httpClient.GetFromJsonAsync<PostViewModel>("Posts/GetByIdPost/" + postId);

            return post != null;
        }
    }
}
