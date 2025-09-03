using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces
{
    public interface IUserPlantRepository :  IRepository<UserPlant>
    {
        Task<IEnumerable<UserPlant>> GetUserPlantsAsync(int userId);
        Task<UserPlant?> GetUserPlantByDeviceIdAsync(int deviceId);
        Task<bool> UserPlantExistsAsync(int userId, int plantId, int deviceId);
        Task<IEnumerable<UserPlant>> GetUserPlantsByUserIdAsync(int userId);
    }
}