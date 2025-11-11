using Microsoft.EntityFrameworkCore;
using ProjectDaedalus.Core.Entities;
using ProjectDaedalus.Core.Interfaces;
using ProjectDaedalus.Infrastructure.Data;

namespace ProjectDaedalus.Infrastructure.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(DaedalusContext context) : base(context){}

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _dbSet.Include(n => n.UserPlant)
                .ThenInclude(up=> up.Plant)
                .Where(n => n.UserPlant.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsByUserIdAsync(int userId)
        {
            return await _dbSet.Include(n => n.UserPlant)
                .ThenInclude(up => up.Plant)
                .Where(n => n.UserPlant.UserId == userId && !n.IsRead)
                .ToListAsync();
        }


        public async Task<int> GetNotificationsCountAsync(int userId)
        {
            return await _dbSet.Include(n => n.UserPlant.User)
                .Where(n=> n.UserPlant.UserId == userId && !n.IsRead)
                .CountAsync();
        }
    }
}

