using AmbalajliyoMVC.ViewModels;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace AmbalajliyoMVC.Service
{
    public class CustomerAPIService
    {
        private readonly HttpClient _httpClient;

        public CustomerAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
        }

        public async Task<List<CustomerViewModel>> GetAllCustomersAsync()
        {
            var response = await _httpClient.GetAsync("Customer/GetAllCustomers");
            if (response.IsSuccessStatusCode)
            {
                var customers = await response.Content.ReadAsAsync<List<CustomerViewModel>>();
                return customers;
            }
            return new List<CustomerViewModel>();
        }

        public async Task CreateCustomerAsync(CustomerViewModel customerViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(customerViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Customer/CreateCustomer", content); // Müşteri ekleme endpoint'i

                response.EnsureSuccessStatusCode(); // Hata kontrolü
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

        public async Task<CustomerViewModel> GetCustomerByIdAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentException("Customer ID cannot be null or empty", nameof(customerId));
            }

            var response = await _httpClient.GetAsync($"Customer/GetCustomerById/{customerId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null; // ID'ye sahip müşteri bulunamadı
                }

                response.EnsureSuccessStatusCode(); // Diğer hatalar için hata fırlat
            }

            var customer = await response.Content.ReadFromJsonAsync<CustomerViewModel>();

            if (customer == null)
            {
                throw new Exception("Customer data could not be deserialized from response.");
            }

            return customer;
        }

        public async Task DeleteCustomerAsync(string customerId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Customer/DeleteCustomer/{customerId}");

                response.EnsureSuccessStatusCode(); // Hata kontrolü
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

        public async Task UpdateCustomerAsync(string customerId, CustomerViewModel customerViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(customerViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"Customer/UpdateCustomer/{customerId}", content);

                response.EnsureSuccessStatusCode(); // Hata kontrolü
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

        public async Task<bool> CustomerExists(string customerId)
        {
            var customer = await _httpClient.GetFromJsonAsync<CustomerViewModel>("Customer/GetCustomerById/" + customerId);

            return customer != null;
        }
        public async Task<List<CustomerViewModel>> GetAllIadesAsync()
        {
            var response = await _httpClient.GetAsync("Customer/GetIades");
            if (response.IsSuccessStatusCode)
            {
                var customers = await response.Content.ReadAsAsync<List<CustomerViewModel>>();
                return customers;
            }
            return new List<CustomerViewModel>();
        }
        public async Task<List<CustomerViewModel>> GetAllSikayetsAsync()
        {
            var response = await _httpClient.GetAsync("Customer/GetIades");
            if (response.IsSuccessStatusCode)
            {
                var customers = await response.Content.ReadAsAsync<List<CustomerViewModel>>();
                return customers;
            }
            return new List<CustomerViewModel>();
        }
        public async Task<List<CustomerViewModel>> GetAllSiparisAsync()
        {
            var response = await _httpClient.GetAsync("Customer/GetAllSiparis");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
              
                var customers = JsonConvert.DeserializeObject<List<CustomerViewModel>>(content);
                return customers;
            }
            return new List<CustomerViewModel>();
        }

    }
}
