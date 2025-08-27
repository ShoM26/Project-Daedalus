using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces
{
    public interface IDeviceRepository : IRepository<Device>
    {
        Task<Device?> GetByDeviceIdAsync(string deviceId);
        Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId);
    }
}

