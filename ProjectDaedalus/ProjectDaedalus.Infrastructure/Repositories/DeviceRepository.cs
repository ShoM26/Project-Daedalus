using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class DeviceRepository : Repository<Device>,  IDeviceRepository
{
    public DeviceRepository(DaedalusContext context) : base(context){}

    public async Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId)
    {
        return await _dbSet.Where(d => d.UserId == userId).ToListAsync();
    }

    public async Task<Device?> GetDeviceByHardwareIdentifierAsync(string hardwareIdentifier)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.HardwareIdentifier == hardwareIdentifier);
    }

    public bool IsDeviceOnline(int deviceId)
    {
        return _dbSet.FirstOrDefault(d => d.DeviceId == deviceId) is { Status: "Online" };
    }
}