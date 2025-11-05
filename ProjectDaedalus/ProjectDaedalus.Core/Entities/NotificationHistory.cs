using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDaedalus.Core.Entities
{
    [Table("notification_history")]
    public class NotificationHistory
    {
        [Key]
        [Column("notification_history_id")]
        public int NotificationHistoryId { get; set; }
        
        [Column("user_plant_id")]
        [Required]
        public int UserPlantId { get; set; }
        
        [Column("notification_type")]
        public NotificationType NotificationType { get; set; }
        
        [Column("moisture_value")]
        [Required]
        public decimal MoistureValue { get; set; }
        
        [Column("threshold_value")]
        [Required]
        public decimal ThresholdValue { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        public UserPlant? UserPlant { get; set; }
    
    }
}

