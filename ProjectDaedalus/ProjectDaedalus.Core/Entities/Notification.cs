using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDaedalus.Core.Entities
{
    [Table("notification")]
    public class Notification
    {
        [Key]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        [Column("userplant_id")]
        [Required]
        public int UserPlantId { get; set; }

        [Column("message")]
        [MaxLength(255)]
        [Required]
        public string Message { get; set; }
        [Column("notification_type")]
        public NotificationType NotificationType { get; set; }

        [Column("is_read")] public bool IsRead { get; set; } = false;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        //Navigation properties
        public UserPlant UserPlant { get; set; }
    }
}

public enum NotificationType
{
    LowMoisture = 1,
    HighMoisture = 2,
    SystemAlert = 3
}