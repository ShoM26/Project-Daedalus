using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User plant);
    Task<User> UpdateAsync(User plant);
    Task<bool> DeleteAsync(User plant);
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    
}