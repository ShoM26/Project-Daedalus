using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class PlantRepository : Repository<Plant>, IPlantRepository
{
    public async Task<Plant?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Plant>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Plant> AddAsync(Plant plant)
    {
        throw new NotImplementedException();
    }

    public async Task<Plant> UpdateAsync(Plant plant)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(Plant plant)
    {
        throw new NotImplementedException();
    }

    public async Task<Plant?> GetByScientificNameAsync(string scientificName)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ScientificNameExistAsync(string scientificName)
    {
        throw new NotImplementedException();
    }
}