using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces
{
    public interface IPlantRepository : IRepository<Plant>
    {
        Task<Plant?> GetByScientificNameAsync(string scientificName);
        Task<bool> ScientificNameExistAsync(string scientificName);
    }
}

