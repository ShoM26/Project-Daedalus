using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces;

public interface IUserPlantRepository
{
    Task<UserPlant?> GetByIdAsync(int id);
    Task<IEnumerable<UserPlant>> GetAllAsync();
    Task<UserPlant> AddAsync(UserPlant plant);
    Task<UserPlant> UpdateAsync(UserPlant plant);
    Task<bool> DeleteAsync(UserPlant plant);
    Task<IEnumerable<UserPlant>> GetUserPlantsAsync(int userId);
    Task<UserPlant?> GetUserPlantByDeviceIdAsync(int deviceId);
    Task<bool> UserPlantExistsAsync(int userId, int plantId, int deviceId);
}