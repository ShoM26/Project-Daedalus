using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces
{
    public interface ISensorReadingRepository : IRepository<SensorHistory>
    {
        Task<SensorHistory?> GetLatestReadingByDeviceIdAsync(int deviceId);
        Task<IEnumerable<SensorHistory>> GetReadingsByDeviceIdAsync(int deviceId);
        Task<IEnumerable<SensorHistory>> GetReadingsByDeviceIdAsync(int deviceId, DateTime fromDate, DateTime toDate);
        Task<bool> DeleteOldReadingsAsync(DateTime cutoffDate);
    }
}

