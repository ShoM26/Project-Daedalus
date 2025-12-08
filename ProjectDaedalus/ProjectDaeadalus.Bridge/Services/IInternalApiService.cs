namespace ProjectDaeadalus.Bridge.Services;

public interface IInternalApiService
{
    Task<T> PostAsync<T>(string endpoint, object data);
    Task<T> GetAsync<T>(string endpoint);
    Task<T> PutAsync<T>(string endpoint, object data);
}