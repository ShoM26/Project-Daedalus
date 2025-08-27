using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class SensorReadingRepository : Repository<SensorHistory>, ISensorReadingRepository
{
    public async Task<SensorHistory?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SensorHistory>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<SensorHistory> AddAsync(SensorHistory plant)
    {
        throw new NotImplementedException();
    }

    public async Task<SensorHistory> UpdateAsync(SensorHistory plant)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(SensorHistory plant)
    {
        throw new NotImplementedException();
    }

    public async Task<SensorHistory?> GetLatestReadingByDeviceIdAsync(int deviceId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SensorHistory>> GetReadingsByDeviceIdAsync(int deviceId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SensorHistory>> GetReadingsByDeviceIdAsync(int deviceId, DateTime fromDate, DateTime toDate)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteOldReadingsAsync(DateTime cutoffDate)
    {
        throw new NotImplementedException();
    }
}