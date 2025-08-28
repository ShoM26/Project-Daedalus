using System.Threading.Tasks;

namespace ProjectDaedalus.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IPlantRepository Plants { get; }
        IDeviceRepository Devices { get; }
        ISensorReadingRepository SensorReadings { get; }

        //Int returns number of entities affected
        Task<int> SaveChangesAsync();
        Task<int> CompleteAsync();
        void Dispose();
    }
}