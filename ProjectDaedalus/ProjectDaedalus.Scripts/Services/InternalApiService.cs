using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectDaedalus.API.Dtos.Plant;

namespace ProjectDaedalus.Scripts.Services
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class BulkInsertResult
    {
        public int TotalPlants { get; set; }
        public int SuccessfulRegistrations { get; set; }
        public int FailedRegistrations { get; set; }
        public string ErrorMessage  { get; set; }
        public bool HasErrors => FailedRegistrations > 0 || (!string.IsNullOrEmpty(ErrorMessage) && ErrorMessage != null);
    }

    public interface IInternalApiService
    {
        Task<BulkInsertResult> BulkPlantRegisterAsync(List<PlantDto> plants);
        Task<bool> TestConnectionAsync();
    }

    public class InternalApiService : IInternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InternalApiService> _logger;
        private readonly string _baseApiUrl;

        public InternalApiService(HttpClient httpClient, IConfiguration configuration,
            ILogger<InternalApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseApiUrl = configuration["BaseApiUrl"];
                
            _httpClient.BaseAddress = new Uri(_baseApiUrl);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ProjectDaedalus-Scripts/1.0");
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", configuration["ApiSettings:ApiKey"]);

        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                _logger.LogInformation("Testing connection to {BaseUrl}", _baseApiUrl);

                var response = await _httpClient.GetAsync("api/plants/health");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully connected to {BaseUrl}", _baseApiUrl);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to connect to {BaseUrl} with errorcode {StatusCode}", _baseApiUrl,
                        response.StatusCode);
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to connect to {BaseUrl}", _baseApiUrl);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "unexpected error testing api connectivity");
                return false;
            }
        }

        public async Task<BulkInsertResult> BulkPlantRegisterAsync(List<PlantDto> plants)
        {
            var result = new BulkInsertResult
            {
                TotalPlants = plants.Count
            };

            try
            {
                _logger.LogInformation("Beginning registration of {Count} plants", plants.Count);

                var jsonContent = JsonSerializer.Serialize(plants, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/plants/bulk-register", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<BulkInsertResult>>(responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                    if (apiResponse?.Success == true && apiResponse.Data != null)
                    {
                        result = apiResponse.Data;
                        _logger.LogInformation("Results: {Success} successful, {Failed} failed",
                            result.SuccessfulRegistrations, result.FailedRegistrations);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        result.FailedRegistrations = plants.Count;
                        result.ErrorMessage = apiResponse?.ErrorMessage;
                        _logger.LogError("API request failed with status {StatusCode}, Error content: {ErrorContent}",
                            response.StatusCode, errorContent);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                result.FailedRegistrations = plants.Count;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Network error during bulk registration");
            }
            catch (JsonException ex)
            {
                result.FailedRegistrations = plants.Count;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "JSON error during bulk registration");
            }
            catch (Exception ex)
            {
                result.FailedRegistrations = plants.Count;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "unexpected error during bulk registration");
            }

            return result;
        }
        
    }
}

