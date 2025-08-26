using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces;

public interface ISensorReadingRepository
{
    Task<SensorHistory?> GetByIdAsync(int id);
    Task<IEnumerable<SensorHistory>> GetAllAsync();
    Task<SensorHistory> AddAsync(SensorHistory plant);
    Task<SensorHistory> UpdateAsync(SensorHistory plant);
    Task<bool> DeleteAsync(SensorHistory plant);
    Task<SensorHistory?> GetLatestReadingByDeviceIdAsync(int deviceId);
    Task<IEnumerable<SensorHistory>> GetReadingsByDeviceIdAsync(int deviceId);
    Task<IEnumerable<SensorHistory>> GetReadingsByDeviceIdAsync(int deviceId, DateTime fromDate, DateTime toDate);
    Task<bool> DeleteOldReadingsAsync(DateTime cutoffDate);
}