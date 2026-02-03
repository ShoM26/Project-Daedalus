using System.Net;
using ProjectDaedalus.API.Dtos.Device;

namespace ProjectDaeadalus.Bridge.Services;

public interface IInternalApiService
{
    Task<T> PostAsync<T>(string endpoint, object data);
    Task<T> FirstRegisterDeviceAsync<T>(RegisterDeviceDto data, string token);
    Task<T> GetAsync<T>(string endpoint);
    Task<HttpStatusCode> CheckDeviceStatusAsync(string endpoint);
    Task<T> PutAsync<T>(string endpoint, object data);
}