using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class UserPlantRepository : Repository<UserPlant>, IUserPlantRepository
{
    public UserPlantRepository(DaedalusContext context) : base(context){}
    
    public async Task<IEnumerable<UserPlant>> GetUserPlantsAsync(int userId)
    {
        return  await _dbSet.Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<UserPlant?> GetUserPlantByDeviceIdAsync(int deviceId)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.DeviceId == deviceId);
    }

    public async Task<bool> UserPlantExistsAsync(int userId, int plantId, int deviceId)
    {
        return await _dbSet.AnyAsync
            (p => p.UserId == userId && 
                  p.PlantId == plantId && 
                  p.DeviceId == deviceId);
    }

    public async Task<IEnumerable<Plant>> GetUserPlantsByUserIdAsync(int userId)
    {
        return await _dbSet.Select(p => p.Plant).Where(u => userId == userId).ToListAsync();
    }
}