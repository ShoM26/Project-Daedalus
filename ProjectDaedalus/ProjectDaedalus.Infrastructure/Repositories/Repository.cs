using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    public async Task<T> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<T> AddAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public async Task<T> UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }
}