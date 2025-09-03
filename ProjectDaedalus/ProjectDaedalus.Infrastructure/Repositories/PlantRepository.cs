using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class PlantRepository : Repository<Plant>, IPlantRepository
{
    public PlantRepository(DaedalusContext context) : base(context){}
    public async Task<Plant?> GetByScientificNameAsync(string scientificName)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.ScientificName == scientificName);
    }

    public async Task<bool> ScientificNameExistAsync(string scientificName)
    {
        return await _dbSet.AnyAsync(p => p.ScientificName == scientificName);
    }
}