using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class DeviceRepository : Repository<Device>,  IDeviceRepository
{
    public DeviceRepository(DaedalusContext context) : base(context){}
    public async Task<Device?> GetByDeviceIdAsync(int deviceId)
    {
        return await _dbSet.FirstAsync(d => d.DeviceId == deviceId);
    }

    public async Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId)
    {
        return await _dbSet.Where(d => d.UserId == userId).ToListAsync();
    }

    public async Task<Device> GetDeviceByHardwareIdentifierAsync(string hardwareIdentifier)
    {
        return await _dbSet.FirstAsync(d => d.HardwareIdentifier == hardwareIdentifier);
    }
}