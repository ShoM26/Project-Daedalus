using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces;

public interface IPlantRepository
{
    Task<Plant?> GetByIdAsync(int id);
    Task<IEnumerable<Plant>> GetAllAsync();
    Task<Plant> AddAsync(Plant plant);
    Task<Plant> UpdateAsync(Plant plant);
    Task<bool> DeleteAsync(Plant plant);
    Task<Plant?> GetByScientificNameAsync(string scientificName);
    Task<bool> ScientificNameExistAsync(string scientificName);
}