using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public async Task<User?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<User> AddAsync(User plant)
    {
        throw new NotImplementedException();
    }

    public async Task<User> UpdateAsync(User plant)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(User plant)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }
}