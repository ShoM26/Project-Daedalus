namespace ProjectDaedalus.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IPlantRepository Plants { get; }
        IDeviceRepository Devices { get; }
        ISensorReadingRepository SensorReadings { get; }
        Task<int> SaveChangesAsync();
    }
}