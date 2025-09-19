using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ProjectDaeadalus.Scripts.Services
{
    public class InternalApiService : IInternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public InternalApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _baseUrl = config["ApiSettings:BaseUrl"];
        
            var apiKey = config["InternalApi:BridgeApiKey"];
            _httpClient.DefaultRequestHeaders.Add("X-Internal-API-Key", apiKey);
        }

        public async Task<T> PostManyAsync<T>(string endpoint, IEnumerable<object> data)
        {
            foreach (var item in data)
            {
                var json = JsonSerializer.Serialize(item); 
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseJson);
            }
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();
        
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseJson);
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
            return response.IsSuccessStatusCode;
        }
    }
}

