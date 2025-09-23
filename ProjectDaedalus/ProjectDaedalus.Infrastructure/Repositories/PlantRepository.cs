using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class PlantRepository : Repository<Plant>, IPlantRepository
{
    public PlantRepository(DaedalusContext context) : base(context)
    {
    }

    public async Task<Plant?> GetByScientificNameAsync(string scientificName)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.ScientificName == scientificName);
    }

    public async Task<bool> ScientificNameExistAsync(string scientificName)
    {
        return await _dbSet.AnyAsync(p => p.ScientificName == scientificName);
    }

    public async Task<BulkInsertResult<Plant>> BulkInsertAsync(IEnumerable<Plant> plants)
    {
        var result = new BulkInsertResult<Plant>
        {
            CreatedItems = new List<Plant>()
        };
        var plantList = plants.ToList();

        result.TotalPlants = plantList.Count;

        using var transation = await _context.Database.BeginTransactionAsync();

        try
        {
            var existingScientifcNames = await _context.Plants
                .Where(p => plantList.Select(pl => pl.ScientificName).Contains(p.ScientificName))
                .Select(p => p.ScientificName).ToListAsync();
            var newPlants = new List<Plant>();
            var skippedPlants = 0;
            foreach (var plant in plantList)
            {
                if (existingScientifcNames.Contains(plant.ScientificName))
                {
                    skippedPlants++;
                }
                else
                {
                    newPlants.Add(plant);
                }
            }

            if (newPlants.Any())
            {
                await _context.Plants.AddRangeAsync(newPlants);
                await _context.SaveChangesAsync();

                result.CreatedItems = newPlants;
            }

            result.FailedRegistrations = skippedPlants;
            result.SuccessfulRegistrations = newPlants.Count;

            await transation.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            await transation.RollbackAsync();
            result.SuccessfulRegistrations = 0;
            result.FailedRegistrations = plantList.Count;
            result.ErrorMessage = ex.Message;
            result.CreatedItems = new List<Plant>();
            return result;
        }
    }
}

    
