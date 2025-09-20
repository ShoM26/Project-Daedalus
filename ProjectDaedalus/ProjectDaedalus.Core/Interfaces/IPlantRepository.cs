using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces
{
    public interface IPlantRepository : IRepository<Plant>
    {
        Task<Plant?> GetByScientificNameAsync(string scientificName);
        Task<bool> ScientificNameExistAsync(string scientificName);
        
        Task<BulkInsertResult<Plant>> BulkInsertAsync(IEnumerable<Plant> plants);
    }
}
public class BulkInsertResult<T>
{
    public int TotalPlants { get; set; }
    public int SuccessfulRegistrations { get; set; }
    public int FailedRegistrations { get; set; }
    
    public List<T> CreatedItems { get; set; } = new List<T>();
    public string ErrorMessage { get; set; }
    
    public bool IsSuccess => FailedRegistrations == 0 && SuccessfulRegistrations > 0;
    public bool HasErrors => ErrorMessage.Length == 0 || FailedRegistrations > 0;
}

