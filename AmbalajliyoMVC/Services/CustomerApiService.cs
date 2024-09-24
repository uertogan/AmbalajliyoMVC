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

        /// <summary>
        /// Retrieves all customer items from the API.
        /// </summary>
        /// <returns>A list of <see cref="CustomerViewModel"/> objects.</returns>
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

        /// <summary>
        /// Creates a new customer by sending a POST request to the API.
        /// </summary>
        /// <param name="customerViewModel">The customer item to be created.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task CreateCustomerAsync(CustomerViewModel customerViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(customerViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("Customer/CreateCustomer", content); // Customer creation endpoint
                response.EnsureSuccessStatusCode(); // Check for errors
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
        /// Retrieves a specific customer item by its ID from the API.
        /// </summary>
        /// <param name="customerId">The ID of the customer item to retrieve.</param>
        /// <returns>The <see cref="CustomerViewModel"/> object with the specified ID, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the customer ID is null or empty.</exception>
        /// <exception cref="Exception">Thrown if an error occurs or if the data cannot be deserialized.</exception>
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
                    return null; // Customer not found
                }
                response.EnsureSuccessStatusCode(); // Throw for other errors
            }

            var customer = await response.Content.ReadFromJsonAsync<CustomerViewModel>();

            if (customer == null)
            {
                throw new Exception("Customer data could not be deserialized from response.");
            }

            return customer;
        }

        /// <summary>
        /// Deletes a customer item by its ID by sending a DELETE request to the API.
        /// </summary>
        /// <param name="customerId">The ID of the customer item to delete.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task DeleteCustomerAsync(string customerId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Customer/DeleteCustomer/{customerId}");
                response.EnsureSuccessStatusCode(); // Check for errors
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
        /// Updates an existing customer item by sending a PUT request to the API.
        /// </summary>
        /// <param name="customerId">The ID of the customer item to update.</param>
        /// <param name="customerViewModel">The updated customer item data.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task UpdateCustomerAsync(string customerId, CustomerViewModel customerViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(customerViewModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"Customer/UpdateCustomer/{customerId}", content);
                response.EnsureSuccessStatusCode(); // Check for errors
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
        /// Checks if a customer exists by sending a GET request to the API.
        /// </summary>
        /// <param name="customerId">The ID of the customer to check.</param>
        /// <returns>True if the customer exists; otherwise, false.</returns>
        public async Task<bool> CustomerExists(string customerId)
        {
            var customer = await _httpClient.GetFromJsonAsync<CustomerViewModel>("Customer/GetCustomerById/" + customerId);
            return customer != null;
        }
    }
}
