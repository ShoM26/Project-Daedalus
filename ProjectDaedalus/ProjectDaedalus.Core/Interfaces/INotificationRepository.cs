using ProjectDaedalus.Core.Entities;

namespace ProjectDaedalus.Core.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<int> GetNotificationsCountAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsByUserIdAsync(int userId);
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId);
    }
}