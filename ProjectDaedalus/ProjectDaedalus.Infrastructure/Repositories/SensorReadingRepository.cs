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
        var deviceExists = _context.Devices.AnyAsync(s => s.DeviceId == deviceId);
        if (deviceExists == null)
        {
            return null;
        }
        return await _dbSet.Where(s => s.DeviceId == deviceId).OrderByDescending(s => s.TimeStamp).FirstAsync();
    }

    public async Task<IEnumerable<SensorHistory>> GetReadingsByDeviceIdAsync(int deviceId)
    {
        return await _context.SensorHistories.Include(sh => sh.Device).Where(s => s.DeviceId == deviceId).OrderByDescending(sh => sh.TimeStamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<SensorHistory>> GetReadingsForDeviceByRangeAsync(int deviceId, DateTime startDate, DateTime endDate)
    {
        return await _context.SensorHistories
            .Include(sh=> sh.Device).Where(sr => sr.DeviceId == deviceId 
                         && sr.TimeStamp >= startDate 
                         && sr.TimeStamp <= endDate)
            .OrderBy(sr => sr.TimeStamp)
            .ToListAsync();
    }

    public async Task<int> DeleteOldReadingsAsync(DateTime cutoffDate)
    {
        var deletedCount = await _dbSet.Include(sh => sh.Device)
            .Where(s => s.TimeStamp < cutoffDate)
            .ExecuteDeleteAsync();
    
        return deletedCount;
    }
}