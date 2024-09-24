using AmbalajliyoMVC.ViewModels;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace AmbalajliyoMVC.Services
{
    public class FaqAPIService
    {
        private readonly HttpClient _httpClient;

        public FaqAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves all FAQ items from the API.
        /// </summary>
        /// <returns>A list of <see cref="FaqViewModel"/> objects.</returns>
        public async Task<List<FaqViewModel>> GetAllFaqAsync()
        {
            var response = await _httpClient.GetAsync("Faq/GetAllFaqs");
            if (response.IsSuccessStatusCode)
            {
                var faqs = await response.Content.ReadAsAsync<List<FaqViewModel>>();
                return faqs;
            }
            return new List<FaqViewModel>();
        }

        /// <summary>
        /// Retrieves a specific FAQ item by its ID from the API.
        /// </summary>
        /// <param name="faqId">The ID of the FAQ item to retrieve.</param>
        /// <returns>The <see cref="FaqViewModel"/> object with the specified ID, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the FAQ ID is null or empty.</exception>
        /// <exception cref="Exception">Thrown if an error occurs or if the data cannot be deserialized.</exception>
        public async Task<FaqViewModel> GetByIdFaqAsync(string faqId)
        {
            if (string.IsNullOrEmpty(faqId))
            {
                throw new ArgumentException("Faq ID cannot be null or empty", nameof(faqId));
            }

            var response = await _httpClient.GetAsync($"Faq/GetFaqById/{faqId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null; // FAQ not found
                }

                response.EnsureSuccessStatusCode(); // Throw for other errors
            }

            var faq = await response.Content.ReadFromJsonAsync<FaqViewModel>();

            if (faq == null)
            {
                throw new Exception("Faq data could not be deserialized from response.");
            }

            return faq;
        }

        /// <summary>
        /// Creates a new FAQ item by sending a POST request to the API.
        /// </summary>
        /// <param name="faqViewModel">The FAQ item to be created.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task CreateFaqAsync(FaqViewModel faqViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(faqViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("Faq/CreateFaq", content);  // FAQ creation endpoint

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
        /// Updates an existing FAQ item by sending a PUT request to the API.
        /// </summary>
        /// <param name="faqId">The ID of the FAQ item to update.</param>
        /// <param name="faqViewModel">The updated FAQ item data.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task UpdateFaqAsync(string faqId, FaqViewModel faqViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(faqViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"Faq/UpdateFaq/{faqId}", content);

                // Ensure that the response indicates success
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
        /// Deletes an FAQ item by its ID by sending a DELETE request to the API.
        /// </summary>
        /// <param name="faqId">The ID of the FAQ item to delete.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task DeleteFaqAsync(string faqId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Faq/DeleteFaq/{faqId}");

                // Ensure that the response indicates success
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
        /// Checks if an FAQ exists by sending a GET request to the API.
        /// </summary>
        /// <param name="faqId">The ID of the FAQ to check.</param>
        /// <returns>True if the FAQ exists; otherwise, false.</returns>
        public async Task<bool> FaqExists(string faqId)
        {
            var faq = await _httpClient.GetFromJsonAsync<FaqViewModel>("Faq/GetFaqById/" + faqId);
            return faq != null;
        }
    }
}
