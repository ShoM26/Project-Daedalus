using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class DeviceRepository : Repository<Device>,  IDeviceRepository
{
    public async Task<Device?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Device>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Device> AddAsync(Device plant)
    {
        throw new NotImplementedException();
    }

    public async Task<Device> UpdateAsync(Device plant)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(Device plant)
    {
        throw new NotImplementedException();
    }

    public async Task<Device?> GetByDeviceIdAsync(string deviceId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }
}