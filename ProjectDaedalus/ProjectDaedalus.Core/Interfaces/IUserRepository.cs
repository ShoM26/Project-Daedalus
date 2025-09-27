using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces
{
    public interface IUserRepository :  IRepository<User>
    {
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> ValidateUserCredentialsAsync(string username, string password);
    }
}

