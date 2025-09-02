using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces
{
    public interface IDeviceRepository : IRepository<Device>
    {
        Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId);
        
        Task<Device> GetDeviceByHardwareIdentifierAsync(string hardwareIdentifier);
    }
}

