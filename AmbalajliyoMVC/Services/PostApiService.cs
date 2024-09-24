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

        /// <summary>
        /// Retrieves all posts from the API.
        /// </summary>
        /// <returns>A list of <see cref="PostViewModel"/> objects.</returns>
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

        /// <summary>
        /// Creates a new post by sending a POST request to the API.
        /// </summary>
        /// <param name="postViewModel">The post data to be created.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task CreatePostAsync(PostViewModel postViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(postViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Posts/AddPost", content);  // Endpoint for adding a post

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
        /// Retrieves a specific post by its ID from the API.
        /// </summary>
        /// <param name="postId">The ID of the post to retrieve.</param>
        /// <returns>The <see cref="PostViewModel"/> object with the specified ID, or null if not found.</returns>
        /// <exception cref="ArgumentException">Thrown if the post ID is null or empty.</exception>
        /// <exception cref="Exception">Thrown if an error occurs or if the data cannot be deserialized.</exception>
        public async Task<PostViewModel> GetByIdPostAsync(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                throw new ArgumentException("Product ID cannot be null or empty", nameof(postId));
            }

            var response = await _httpClient.GetAsync($"Posts/GetByIdPost/{postId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null; // Post not found
                }

                response.EnsureSuccessStatusCode(); // Throw for other errors
            }

            var post = await response.Content.ReadFromJsonAsync<PostViewModel>();

            if (post == null)
            {
                throw new Exception("Post data could not be deserialized from response.");
            }

            return post;
        }

        /// <summary>
        /// Deletes a post by its ID by sending a DELETE request to the API.
        /// </summary>
        /// <param name="postId">The ID of the post to delete.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task DeletePostAsync(string postId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Posts/DeletePost/{postId}");

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
        /// Updates an existing post by sending a PUT request to the API.
        /// </summary>
        /// <param name="postId">The ID of the post to update.</param>
        /// <param name="postViewModel">The updated post data.</param>
        /// <exception cref="Exception">Thrown if an error occurs during the request.</exception>
        public async Task UpdatePostAsync(string postId, PostViewModel postViewModel)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(postViewModel), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"Posts/UpdatePost/{postId}", content);

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
        /// Checks if a post exists by sending a GET request to the API.
        /// </summary>
        /// <param name="postId">The ID of the post to check.</param>
        /// <returns>True if the post exists; otherwise, false.</returns>
        public async Task<bool> PostExists(string postId)
        {
            var post = await _httpClient.GetFromJsonAsync<PostViewModel>("Posts/GetByIdPost/" + postId);
            return post != null;
        }
    }
}
