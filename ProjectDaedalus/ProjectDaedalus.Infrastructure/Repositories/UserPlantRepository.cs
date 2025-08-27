using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class UserPlantRepository : Repository<UserPlant>, IUserPlantRepository
{
    public async Task<UserPlant?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserPlant>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<UserPlant> AddAsync(UserPlant plant)
    {
        throw new NotImplementedException();
    }

    public async Task<UserPlant> UpdateAsync(UserPlant plant)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(UserPlant plant)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserPlant>> GetUserPlantsAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<UserPlant?> GetUserPlantByDeviceIdAsync(int deviceId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UserPlantExistsAsync(int userId, int plantId, int deviceId)
    {
        throw new NotImplementedException();
    }
}