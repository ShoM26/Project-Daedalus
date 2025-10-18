using System.Threading.Tasks;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DaedalusContext _context;

        public IUserRepository Users { get; }
        public IPlantRepository Plants { get; }
        public IDeviceRepository Devices { get; }
        public ISensorReadingRepository SensorReadings { get; }

        public UnitOfWork(
            DaedalusContext context,
            IUserRepository users,
            IPlantRepository plants,
            IDeviceRepository devices,
            ISensorReadingRepository sensorReadings)
        {
            _context = context;
            Users = users;
            Plants = plants;
            Devices = devices;
            SensorReadings = sensorReadings;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}