namespace ProjectDaedalus.Core.Entities;

public class UserPlant
{
    public int UserPlantId { get; set; }

    public int UserId { get; set; }

    public int PlantId { get; set; }

    public int DeviceId { get; set; }

    public DateTime? DateAdded { get; set; }

    public virtual Device Device { get; set; }

    public virtual Plant Plant { get; set; }

    public virtual User User { get; set; }
    
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<NotificationHistory> NotificationHistories { get; set; } = new List<NotificationHistory>();
    
}
