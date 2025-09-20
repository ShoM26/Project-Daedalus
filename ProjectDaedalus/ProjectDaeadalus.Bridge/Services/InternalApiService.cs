using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ProjectDaeadalus.Bridge.Services
{
    public class InternalApiService : IInternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public InternalApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _baseUrl = config["ApiSettings:BaseUrl"];
        
            var apiKey = config["ApiSettings:ApiKey"];
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
            var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
            response.EnsureSuccessStatusCode();
        
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseJson);
        }
    }
}

