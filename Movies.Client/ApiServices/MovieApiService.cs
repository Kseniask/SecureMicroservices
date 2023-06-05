using IdentityModel.Client;
using Movies.Client.Models;
using Newtonsoft.Json;

namespace Movies.Client.ApiServices
{
    public class MovieApiService : IMovieApiService
    {
        private IHttpClientFactory _httpClientFactory;

        public MovieApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<Movie>> GetMovies()
        {
            // WAY 1
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/");
            var content = await GetContent(request);

            List<Movie> movieList = JsonConvert.DeserializeObject<List<Movie>>(content);

            return movieList;

            /* WAY 2 
            // Get token from IDS
            var apiClientCredentials = new ClientCredentialsTokenRequest
            {
                Address = "https://localhost:5005/connect/token",
                ClientId = "movieClient",
                ClientSecret = "secret",
                Scope = "movieAPI"
            };

            // create http client to talk to IDS
            var client = new HttpClient();

            var discover = await client.GetDiscoveryDocumentAsync("https://localhost:5005");
            if(discover.IsError) 
            {
                throw new Exception("descovery endpoint failed");
            }

            // authenticate and GET access token

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
            if (tokenResponse.IsError)
            {
                throw new Exception("token endpoint failed");
            }

            // Send request to protect API (Movies API)

            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://localhost:5001/api/movies");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            // Deserialize object to MovieList
            List<Movie> movieList = JsonConvert.DeserializeObject<List<Movie>>(content);

            return movieList;*/
        }

        public async Task<Movie> GetMovie(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/{id}");
            var content = await GetContent(request);

            return JsonConvert.DeserializeObject<Movie>(content);
        }

        public async Task<Movie> CreateMovie(Movie movie)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/movies");
            request.Content = JsonContent.Create(movie);
            var content = await GetContent(request);

            return JsonConvert.DeserializeObject<Movie>(content);
        }

        public async Task<Movie> UpdateMovie(Movie movie, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/movies/{id}");
            request.Content = JsonContent.Create(movie);
            var content = await GetContent(request);

            return JsonConvert.DeserializeObject<Movie>(content);
        }

        public async Task DeleteMovie(int id)
        {
           
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/movies/{id}");
            await GetContent(request);
        }

        private async Task<string> GetContent(HttpRequestMessage request)
        {
            var httpClient = _httpClientFactory.CreateClient("MovieAPIClient");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
