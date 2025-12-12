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

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // This fixes the CamelCase vs PascalCase issue
        };

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
        }
        
        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            Console.WriteLine($"Sending to API: {json}");
            var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API call failed: {response.StatusCode} - {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
        }
    }
}

