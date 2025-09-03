using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DaedalusContext context) : base(context){}

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(p => p.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return  await _dbSet.FirstOrDefaultAsync(p => p.Username == username);
    }
}