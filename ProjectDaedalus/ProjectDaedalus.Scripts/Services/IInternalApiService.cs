namespace ProjectDaeadalus.Scripts.Services;

public interface IInternalApiService
{
    Task<T> PostManyAsync<T>(string endpoint, IEnumerable<object> data);
    Task<T> GetAsync<T>(string endpoint);
    Task<bool> DeleteAsync(string endpoint);
}