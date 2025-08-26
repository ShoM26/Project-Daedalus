using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces;

public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(int id);
    Task<IEnumerable<Device>> GetAllAsync();
    Task<Device> AddAsync(Device plant);
    Task<Device> UpdateAsync(Device plant);
    Task<bool> DeleteAsync(Device plant);
    Task<Device?> GetByDeviceIdAsync(string deviceId);
    Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId);
}