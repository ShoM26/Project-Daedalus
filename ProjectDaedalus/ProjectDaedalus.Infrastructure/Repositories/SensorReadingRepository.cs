using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class SensorReadingRepository : Repository<SensorHistory>, ISensorReadingRepository
{
    public SensorReadingRepository(DaedalusContext context) : base(context){}
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