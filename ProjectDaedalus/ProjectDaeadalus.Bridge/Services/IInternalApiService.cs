namespace ProjectDaeadalus.Bridge.Services;

public interface IInternalApiService
{
    Task<T> PostAsync<T>(string endpoint, object data);
}